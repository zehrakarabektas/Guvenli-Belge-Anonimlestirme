import spacy
import re
nlp = spacy.load("en_core_web_lg")

aranacakCumleler = [
    "He received the degree from the institution.",
    "She is currently working at the university.",
    "He is a student at the institution.",
    "He is affiliated with the university.",
    "She is pursuing a degree from the college."
]

def yazarBaglantilimi(sentence):
    kelimeler = ["received", "working", "student", "affiliated", "pursuing", "degree", "graduated", "currently at"]
    sentence_lower = sentence.lower()
    
    if any(kw in sentence_lower for kw in kelimeler):
        dokuman = nlp(sentence)
        for cumle in aranacakCumleler:
            dokuman2 = nlp(cumle)
            if dokuman.similarity(dokuman2) > 0.80:
                return True
    return False


def iptal(page, keyword):
    text = page.get_text()
    sentences = re.split(r'(?<=[.!?]) +', text)
    for sentence in sentences:
        if keyword in sentence:
            return sentence
    return ""

def semantikKurumTespiti(doc):
    kurumlar = []

    for page in doc:
        text = page.get_text()
        spacyDokuman = nlp(text)

        for sent in spacyDokuman.sents:
            if yazarBaglantilimi(sent.text):
                text = nlp(sent.text)  
                for ent in text.ents:
                    if ent.label_ == "ORG" and len(ent.text.strip()) > 5:
                        anlamli = [token.text for token in ent if token.pos_ == "PROPN"]
                        if anlamli:
                            proper_text = " ".join(anlamli).strip()
                            kurumlar.append((page, proper_text))
                            print(f"[DEBUG] {ent.text} → {proper_text}")
    return kurumlar

