from fastapi import FastAPI, UploadFile, File,Form
from fastapi.staticfiles import StaticFiles
from fastapi.responses import JSONResponse
import uvicorn
import fitz  
import re
import uuid
from ArticleFieldTopic import articleTopicAnalizi
from ArticleBilgiTespit import  yazarBilgiMetniBul,yazarAdTespiti,yazarKurumTespiti,yazarMailTespiti,generate_name_variants
from ArticleAnlamsalAnaliz import semantikKurumTespiti
import numpy as np
import os
import base64
from PIL import Image, ImageFilter
import io
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad, unpad
from Crypto.Random import get_random_bytes
import json

app = FastAPI()

def pdftotext(file):
    pdf_document = fitz.open(stream=file, filetype="pdf")
    text = ""
    for page_num in range(pdf_document.page_count):
        page = pdf_document.load_page(page_num)
        text += page.get_text()
    return text

@app.post("/analyze/")
async def analyze_file(file: UploadFile = File(...)):
    content = await file.read()
    text = pdftotext(content)
    return JSONResponse(content={"length": len(text), "preview": text[:55000]})

def getAbstractKeyword(text):
    text = re.sub(r"\s+", " ", text)
    abstract_pattern = re.compile(
        r"\babstract\b[\s:\-–—]*\s*(.+?)\s*(?=\b(keywords|index terms)\b|\b1\.|introduction|i\.)",
        re.IGNORECASE | re.DOTALL
    )
    keywords_pattern = re.compile(
        r"\b(keywords|index terms)\b[\s:\-–—]*\s*(.+?)(?=\n{2,}|\b1\.|introduction|i\.|\Z)",
        re.IGNORECASE | re.DOTALL
    )
    abstractEslesme = abstract_pattern.search(text)
    keywordsEslesme  = keywords_pattern.search(text)
    abstract = abstractEslesme.group(1).strip() if abstractEslesme else ""
    keywords_raw = keywordsEslesme .group(2).strip() if keywordsEslesme else ""
    keywords = re.split(r"[;,]", keywords_raw)
    list = {"component", "keywords", "keyword", "index terms"}
    keywords = [k.strip().lower() for k in keywords if k.strip().lower() not in list]

    return abstract, keywords

@app.post("/abstractAndKeyword/")
async def pdfAbstractKeyword(file: UploadFile = File(...)):
    content = await file.read()
    text = pdftotext(content)  
    abstract, keywords = getAbstractKeyword(text)
    return JSONResponse(content={
        "abstract": abstract,
        "keywords": keywords
    })

@app.post("/articleTopic/")
async def analyzePdfFieldTTopic(file: UploadFile = File(...)):
    content = await file.read()
    text = pdftotext(content)

    abstract, keywords = getAbstractKeyword(text)
    birlestirilmistext = abstract + " " + " ".join(keywords)

    topics = articleTopicAnalizi(birlestirilmistext)
    return JSONResponse(content= topics)

@app.post("/ArticleYazarBolumu/")
async def article_yazar_bolumu(file: UploadFile = File(...)):
    content = await file.read()
    doc = fitz.open(stream=content, filetype="pdf")

    authorbilgitext =yazarBilgiMetniBul(doc)
    doc.close()
    emails, gerikalantext = yazarMailTespiti(authorbilgitext)
    kurumBilgi, gerikalantext1 = yazarKurumTespiti(gerikalantext)
    yazarlar = yazarAdTespiti(gerikalantext1)

    return JSONResponse({
        "author_block": authorbilgitext,
        "text":gerikalantext,
        "text1":gerikalantext1,
        "emails": emails,
        "institutions": kurumBilgi,
        "authors": yazarlar
    })

os.makedirs("anonymized_pdfs", exist_ok=True)
app.mount("/files", StaticFiles(directory="anonymized_pdfs"), name="files")
def encrypt_aes(data_dict):
    key = get_random_bytes(16)
    nonce = get_random_bytes(12)
    cipher = AES.new(key, AES.MODE_GCM, nonce=nonce)

    data_str = json.dumps(data_dict)
    ciphertext, tag = cipher.encrypt_and_digest(data_str.encode("utf-8"))

    encrypted_package = {
        "key": base64.b64encode(key).decode("utf-8"),
        "nonce": base64.b64encode(nonce).decode("utf-8"),
        "ciphertext": base64.b64encode(ciphertext).decode("utf-8"),
        "tag": base64.b64encode(tag).decode("utf-8")
    }
    return encrypted_package

def decrypt_aes(encrypted_package):
    key = base64.b64decode(encrypted_package["key"])
    nonce = base64.b64decode(encrypted_package["nonce"])
    ciphertext = base64.b64decode(encrypted_package["ciphertext"])
    tag = base64.b64decode(encrypted_package["tag"])

    cipher = AES.new(key, AES.MODE_GCM, nonce=nonce)
    decrypted = cipher.decrypt_and_verify(ciphertext, tag)
    return json.loads(decrypted.decode("utf-8"))
def encrypt_bytes_aes(data_bytes, key):
    iv = get_random_bytes(16)
    cipher = AES.new(key, AES.MODE_CFB, iv=iv)
    encrypted = cipher.encrypt(data_bytes)
    return {
        "iv": base64.b64encode(iv).decode("utf-8"),
        "data": base64.b64encode(encrypted).decode("utf-8")
    }
def decrypt_image_gcm(enc_data):
    key = base64.b64decode(enc_data["key"])
    nonce = base64.b64decode(enc_data["nonce"])
    ciphertext = base64.b64decode(enc_data["ciphertext"])
    tag = base64.b64decode(enc_data["tag"])

    cipher = AES.new(key, AES.MODE_GCM, nonce=nonce)
    decrypted_bytes = cipher.decrypt_and_verify(ciphertext, tag)
    return decrypted_bytes


def encrypt_image_gcm(image_bytes, image_format="PNG"):
    key = get_random_bytes(16)
    cipher = AES.new(key, AES.MODE_GCM)
    ciphertext, tag = cipher.encrypt_and_digest(image_bytes)

    return {
        "ciphertext": base64.b64encode(ciphertext).decode("utf-8"),
        "tag": base64.b64encode(tag).decode("utf-8"),
        "nonce": base64.b64encode(cipher.nonce).decode("utf-8"),
        "key": base64.b64encode(key).decode("utf-8"),
        "format": image_format
    }

def yazarResmi(image_rect, text_blocks, keywords):
    for block in text_blocks:
        block_rect = fitz.Rect(block[:4])
        text = block[4].lower()

        if image_rect.intersects(block_rect):
            for kw in keywords:
                if kw.lower() in text:
                    return True

        horizontally_aligned = (
            abs(block_rect.y1 - image_rect.y0) < 60 or abs(block_rect.y0 - image_rect.y1) < 60
        )
        if horizontally_aligned:
            horizontal_distance = abs(block_rect.x0 - image_rect.x0)
            if horizontal_distance < 200:
                for kw in keywords:
                    if kw.lower() in text:
                        return True

    print("[DEBUG] No matching block near image.")
    return False
def pdfimagedesifre(doc, encrypted_images):
    for i, enc in enumerate(encrypted_images):
        label = enc.get("label")
        page_index = enc.get("page")
        position = enc.get("position")
        enc_data = enc.get("encrypted_image")

        if not position or not enc_data:
            print(f"[DEBUG] Görsel {label} — eksik bilgi, atlanıyor.")
            continue

        try:
            decrypted_bytes = decrypt_image_gcm(enc_data)

            image = Image.open(io.BytesIO(decrypted_bytes))
            image_format = enc_data.get("format", "PNG")

            image_stream = io.BytesIO()
            image.save(image_stream, format=image_format)
            image_stream.seek(0)

            rect = fitz.Rect(position["x0"], position["y0"], position["x1"], position["y1"])
            page = doc[page_index]
            page.insert_image(rect, stream=image_stream.read())

            print(f"[DEBUG]  {label} geri yüklendi. Sayfa {page_index}, Konum: {rect}")
        except Exception as e:
            print(f"[ERROR] Görsel {label} geri yüklenemedi: {e}")

    return doc

def orjinalResmiKaydet(image_bytes, save_folder="original_images"):
    os.makedirs(save_folder, exist_ok=True)
    image_id = str(uuid.uuid4())
    save_path = os.path.join(save_folder, f"{image_id}.png")
    with open(save_path, "wb") as f:
        f.write(image_bytes)
    return save_path

def blurlaimage(doc, keywords):
    page = doc[-1]
    images = page.get_images(full=True)
    text_blocks = page.get_text("blocks")
    encrypted_images = []

    for img_index, img_info in enumerate(images):
        xref = img_info[0]
        base_image = doc.extract_image(xref)
        image_bytes = base_image["image"]

        if base_image["width"] < 60 or base_image["height"] < 60:
            continue

        bbox = fitz.Rect(img_info[1], img_info[2], img_info[3], img_info[4])
        print(f"[DEBUG] Görsel {img_index} konumu: {bbox}")

        image = Image.open(io.BytesIO(image_bytes))
        image_format = image.format or "PNG"
        encrypted_data = encrypt_image_gcm(image_bytes, image_format)

        blurred_image = image.filter(ImageFilter.GaussianBlur(radius=15))
        blurred_buffer = io.BytesIO()
        blurred_image.save(blurred_buffer, format=image_format)
        blurred_bytes = blurred_buffer.getvalue()

        page.replace_image(xref, stream=blurred_bytes)

        
        encrypted_images.append({
            "label": f"Resim{img_index + 1}",
            "page": doc.page_count - 1,
            "position": {
                "x0": bbox.x0,
                "y0": bbox.y0,
                "x1": bbox.x1,
                "y1": bbox.y1
            },
            "encrypted_image": encrypted_data
        })

    return doc, encrypted_images


def yazarismibul(page, full_name):
    words = page.get_text("words")
    name_parts = full_name.split()
    results = []

    for i in range(len(words)):
        match = True
        temp_rects = []
        for j in range(len(name_parts)):
            if i + j >= len(words):
                match = False
                break
            word_text = re.sub(r"[^\w]", "", words[i + j][4]).lower()
            if word_text != name_parts[j].lower():
                match = False
                break
            temp_rects.append(fitz.Rect(*words[i + j][0:4]))

        if match:
            combined = temp_rects[0]
            for r in temp_rects[1:]:
                combined |= r
            results.append(combined)

    return results

def referanskodu(text, text_type, reference_map, counter):
    for code, info in reference_map.items():
        if info["value"] == text and info["type"] == text_type:
            return code

    ref_code = f"#{text_type.upper()}{counter[text_type]:02d}"
    counter[text_type] += 1
    reference_map[ref_code] = {"type": text_type, "value": text}
    return ref_code

def bulreferanskodunuekle(page, text, text_type, original_values, reference_map, counter):
    matched_rects = page.search_for(text)
    if not matched_rects:
        matched_rects = yazarismibul(page, text)

    final_rects = []

    for rect in matched_rects:
        ref_code = referanskodu(text, text_type, reference_map, counter)

        if not any(item["type"] == text_type and item["value"] == text and item["ref_code"] == ref_code for item in original_values):
            original_values.append({"type": text_type, "value": text, "ref_code": ref_code})

        page.add_redact_annot(rect, fill=(1, 1, 1))
        final_rects.append((rect, ref_code))

    if final_rects:
        page.apply_redactions()

    for rect, ref_code in final_rects:
        font_size = min(max(rect.y1 - rect.y0 - 1, 5), 6)
        page.insert_text(
            point=(rect.x0, rect.y1 - 1),
            text=ref_code,
            fontsize=font_size,
            fontname="helv",
            color=(0, 0, 0)
        )

@app.post("/anonymize_pdf/")
async def anonymize_pdf(
    file: UploadFile = File(...),
    anon_ad: bool = Form(...),
    anon_email: bool = Form(...),
    anon_kurum: bool = Form(...)
):
    content = await file.read()
    doc = fitz.open(stream=content, filetype="pdf")

    authortext = yazarBilgiMetniBul(doc)
    emails, gerikalantext = yazarMailTespiti(authortext)
    kurumlar, gerikalantext1 = yazarKurumTespiti(gerikalantext)
    names = yazarAdTespiti(gerikalantext1)

    aes_key = get_random_bytes(16)
    keyword=names
    doc, encrypted_images = blurlaimage(doc, keyword)

    reference_map = {}
    original_values = []
    sayac = {"email": 1, "ad": 1, "kurum": 1}

    for page in doc:
        if anon_email:
            for email in emails:
                bulreferanskodunuekle(page, email, "email", original_values, reference_map, sayac)

        if anon_ad:
            for name in names:
                 variants = generate_name_variants(name)
                 for variant in variants:
                        bulreferanskodunuekle(page, variant, "ad", original_values, reference_map, sayac)

        if anon_kurum:
            for kurum in kurumlar:
                bulreferanskodunuekle(page, kurum, "kurum", original_values, reference_map, sayac)
            
            
    if anon_kurum:
        additional_mentions = semantikKurumTespiti(doc)
        for page, kurum in additional_mentions:
            bulreferanskodunuekle(page, kurum, "kurum", original_values, reference_map, sayac)
    pdf_bytes = doc.write()
    doc.close()

    encrypted_info = encrypt_aes(reference_map)

    encrypted_data = {
        "pdf_bytes": base64.b64encode(pdf_bytes).decode("utf-8"),  
        "encrypted_info": encrypted_info,
        "encrypted_images": encrypted_images,
        "aes_key": base64.b64encode(aes_key).decode("utf-8")  
    }

    return JSONResponse(content={"encrypted_data": encrypted_data})


@app.post("/deanonymize_pdf/")
async def deanonymize_pdf(
    file: UploadFile = File(...),
    encrypted_info: str = Form(...)
):
    content = await file.read()
    doc = fitz.open(stream=content, filetype="pdf")
    encrypted_data = json.loads(encrypted_info)

    reference_map = decrypt_aes(encrypted_data["encrypted_info"])
    for page in doc:
        words = page.get_text("words")
        for ref_code, info in reference_map.items():
            for w in words:
                if ref_code == w[4]:
                    rect = fitz.Rect(w[0], w[1], w[2], w[3])
                    page.add_redact_annot(rect, fill=(1, 1, 1))
                    page.apply_redactions()
                    font_size = max(min((rect.y1 - rect.y0) * 0.6, 10), 7)
                    page.insert_text(
                        point=(rect.x0, rect.y1 - 1),
                        text=info["value"],
                        fontsize=font_size,
                        fontname="helv",
                        color=(0, 0, 0)
                    )

    encrypted_images = encrypted_data.get("encrypted_images", [])
    if encrypted_images:
        doc = pdfimagedesifre(doc, encrypted_images)

    pdf_bytes = doc.write()
    doc.close()
    encoded = base64.b64encode(pdf_bytes).decode("utf-8")

    return {"pdf_bytes": encoded}


if __name__ == "__main__":
    uvicorn.run("NlpArticleService:app", host="127.0.0.1", port=8001, reload=True)