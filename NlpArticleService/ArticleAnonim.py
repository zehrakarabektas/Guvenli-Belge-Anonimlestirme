
import spacy
import fitz
import re
nlp = spacy.load("en_core_web_lg")

#def pdfAnonimlestir(pdf, makaleId, yazarAdiAnonim,yazarIletisimAnonim,yazarKurumAnonim)
    #doc = fitz.open(pdf)
    #metadata = {}

    #for page in doc:
        #if yazarAdiAnonim:
            
        #if yazarIletisimAnonim:
        #if yazarKurumAnonim:

    #output_path = f"anonim_{makaleId}.pdf"
    #doc.save(output_path)
    #doc.close()
    #os.remove(pdf_path)

    #return output_path, metadata

def articleAuthorsName(text: str) -> dict:

    lines = [line.strip() for line in text.split("\n") if line.strip()]
    
    title = ""
    abstract = ""
    authors = []

    for i in range(min(8, len(lines))):
        if 20 < len(lines[i]) < 200:
            title += lines[i] + " "
        elif title:
            break  
    title = title.strip()
    
    joined_text = "\n".join(lines)
    abstract_match = re.search(
        r"\babstract\b[\s\-–—:]*([\s\S]+?)(?=\bindex terms\b|\bkeywords\b|\n\s*[I1]\.|\n\n)",
        joined_text,
        re.IGNORECASE
    )
    if abstract_match:
        abstract = abstract_match.group(1).strip()
    
    abstract_index = next((i for i, line in enumerate(lines) if re.search(r"\babstract\b", line, re.IGNORECASE)), -1)

    title_end_index = next((i for i, line in enumerate(lines) if title.strip().endswith(line.strip())), 0)

    author_block = lines[title_end_index + 1 : abstract_index] if 0 < abstract_index > title_end_index else lines[:30]

    name_pattern = re.compile(r"\b[A-Z][a-z]+(?:\s[A-Z]\.)?\s[A-Z][a-z]+\b")
    upper_name_line_pattern = re.compile(r"([A-Z][A-Z\s\-]+)(,|$)")

    candidates = set()

    for line in author_block:
        if line.strip().isupper() and line.count(",") >= 1:
            matches = upper_name_line_pattern.findall(line)
            for m in matches:
                candidates.add(m[0].title())
        else:
            normal_matches = name_pattern.findall(line)
            for match in normal_matches:
                candidates.add(match)

    authors = filter_real_persons(list(candidates))

    return {
        "title": title,
        "abstract": abstract,
        "authors": authors
    }

def extract_title_authors_abstract(doc) -> dict:
    page = doc[0]
    blocks = page.get_text("dict")["blocks"]

    title = ""
    abstract = ""
    authors_raw_lines = []

    font_map = []

    for block in blocks:
        if "lines" not in block:
            continue
        for line in block["lines"]:
            spans = line.get("spans", [])
            if not spans:
                continue
            text = " ".join([s["text"] for s in spans]).strip()
            size = round(spans[0]["size"], 2)
            font_map.append((text, size))

    if not font_map:
        return {"title": "", "abstract": "", "authors": []}

    max_font = max(s for _, s in font_map)
    title_lines = [text for text, size in font_map if abs(size - max_font) < 0.5]
    title = " ".join(title_lines).strip()

    abstract_start = next((i for i, (t, _) in enumerate(font_map) if "abstract" in t.lower()), -1)

    title_end = next((i for i, (t, s) in enumerate(font_map) if t.strip() == title_lines[-1].strip()), 0)

    authors_raw_lines = [text for text, _ in font_map[title_end+1:abstract_start] if text.strip()]

    abstract_lines = []
    for i in range(abstract_start + 1, len(font_map)):
        text, _ = font_map[i]
        if re.search(r"\b(index terms|keywords)\b", text.lower()):
            break
        abstract_lines.append(text)
    abstract = " ".join(abstract_lines).strip()

    return {
        "title": title,
        "abstract": abstract,
        "authors_raw": authors_raw_lines
    }

def extract_author_names(lines: list[str]) -> list[str]:
    candidates = set()
    for line in lines:
        if line.isupper() and ("," in line or " AND " in line):
            parts = re.split(r",|\band\b", line, flags=re.IGNORECASE)
            for part in parts:
                name = part.strip()
                if 2 <= len(name.split()) <= 4:
                    candidates.add(name.title())
        matches = re.findall(r"\b(?:[A-Z]\.|[A-Z][a-z]+)(?:\s[A-Z]\.|\\s[A-Z][a-z]+)*\b", line)
        for m in matches:
            candidates.add(m.strip())
    return sorted(candidates)


def filter_real_persons(candidates: list[str]) -> list[str]:
    persons = []
    for name in candidates:
        doc = nlp(name)
        for ent in doc.ents:
            if ent.label_ == "PERSON":
                persons.append(ent.text)
    return list(set(persons))


