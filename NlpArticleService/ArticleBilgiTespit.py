import spacy
import fitz
import re
nlp = spacy.load("en_core_web_trf", disable=["parser", "tagger", "lemmatizer", "attribute_ruler"])

def yazarBilgiMetniBul(doc):
    makaleSayfa = doc[0]
    sayfaBoyutu = makaleSayfa.rect.height
    blocks = makaleSayfa.get_text("dict")["blocks"]
    satir = []

    for block in blocks:
        if "lines" not in block:
            continue
        for line in block["lines"]:
            spans = line.get("spans", [])
            if not spans:
                continue
            text = " ".join([s["text"] for s in spans]).strip()
            size = round(spans[0]["size"], 2)
            y_konum = spans[0]["bbox"][1]
            satir.append((text, size, y_konum))

    if not satir:
        return ""

    max_font = max(size for _, size, _ in satir)
    baslikSatiri = [text for text, size, _ in satir if abs(size - max_font) < 0.5]
    baslik = " ".join(baslikSatiri).strip()

    baslikBitis = 0
    for text, size, y in satir:
        if text.strip() == baslikSatiri[-1].strip():
            baslikBitis = y
            break

    abstractBas = None
    for text, size, y in satir:
        if "abstract" in text.lower():
            abstractBas = y
            break

    if abstractBas is None:
        return ""

    baslikSatirleri = [
        text for text, size, y in satir
        if baslikBitis < y < abstractBas and size < max_font - 0.5 and y < sayfaBoyutu * 0.6
    ]

    return "\n".join(baslikSatirleri).strip()

alinmayacakKelimeler = {
    "corresponding", "author", "member", "graduate", "student", "grant",
    "support", "foundation", "project", "commission", "committee",
    "planning", "fund", "government", "this", "that", "research", "program"
}

def is_valid_author(name: str) -> bool:
    isim = name.lower()
    kelimesay = len(name.split())

    if kelimesay == 1:
        if not name.istitle():
            return False
        if not re.fullmatch(r"[A-Za-z]+", name):
            return False
        if isim in alinmayacakKelimeler:
            return False
        return True

    if kelimesay >= 2:
        if any(blocked in isim for blocked in alinmayacakKelimeler):
            return False
        if not re.fullmatch(r"[A-Za-z\s\.]+", name):
            return False
        return True

    return False


def yazarAdTespiti(text: str) -> list[str]:
    satirler = [line.strip() for line in text.split("\n") if line.strip()]
    adolabilecekler = set()
    for s in satirler:
        if s.isupper():
            parts = re.split(r",| and | AND | & |\band\b", s, flags=re.IGNORECASE)
            for part in parts:
                part = part.strip()
                if is_valid_author(part):
                    adolabilecekler.add(part.title())

        isimRegex = r'\b([A-Z][a-z]+(?:\s[A-Z][a-z]+)+)\b'
        eslesmeler = re.findall(isimRegex, s)
        for e in eslesmeler:
            if is_valid_author(e.strip()):
                adolabilecekler.add(e.strip().title())

    doc = nlp(text)
    for ent in doc.ents:
        if ent.label_ == "PERSON":
            name = ent.text.strip()
            if is_valid_author(name):
                adolabilecekler.add(name.title())

    return sorted(adolabilecekler)


def filter_real_persons(candidates: list[str]) -> list[str]:
    persons = []
    for name in candidates:
        doc = nlp(name)
        for ent in doc.ents:
            if ent.label_ == "PERSON":
                persons.append(ent.text)
    return list(set(persons))

def yazarMailTespiti(text: str) -> tuple[list[str], str]:
    email_pattern = re.compile(r"[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+")
    emails = email_pattern.findall(text)
    cleaned_text = email_pattern.sub('', text)
    return list(dict.fromkeys(emails)), cleaned_text.strip()


def yazarKurumTespiti(text: str) -> tuple[list[str], str]:
    kurumkelimeler = [
        "university", "institute", "department", "faculty", "college", 
        "school", "engineering", "center", "centre", "laboratory"
    ]

    satirlar = [line.strip() for line in text.split('\n') if line.strip()]
    
    affiliations = []
    remaining_lines = []

    for s in satirlar:
        line_lower =s.lower()
        if any(keyword in line_lower for keyword in kurumkelimeler):
            temizlenmis_line = temizle_bas_son_sayi(s)
            affiliations.append(temizlenmis_line)
        else:
            remaining_lines.append(s)

    remaining_text = "\n".join(remaining_lines)

    doc = nlp(text)
    gpe_entities = {ent.text.strip() for ent in doc.ents if ent.label_ == "GPE"}

    for gpe in gpe_entities:
        pattern = r"\b" + re.escape(gpe) + r"\b"
        remaining_text = re.sub(pattern, "", remaining_text)

    for aff in affiliations:
        pattern = re.escape(aff)
        remaining_text = re.sub(pattern, "", remaining_text, flags=re.IGNORECASE)

    return list(dict.fromkeys(affiliations)), remaining_text.strip()

def generate_name_variants(full_name):
    parts = full_name.strip().split()
    if len(parts) < 2:
        return [full_name]  

    first, last = parts[0], parts[-1]
    initials = first[0].upper()

    variants = {
        full_name,                  
        f"{initials}. {last}",      
        f"{initials} {last}",       
        f"{first} {last[0]}."       
    }

    return list(variants)

def temizle_bas_son_sayi(line):
    line = re.sub(r"^\s*\d+\s*", "", line)
    line = re.sub(r"\s*\d+\s*$", "", line)
    return line.strip()
