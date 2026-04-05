from flask import Flask, request, jsonify
import fitz
from ArticleFieldTopic import articleTopicAnalizi

app = Flask(__name__)


@app.route('/ArticleFieldTopicAnaliz', methods=['POST'])
def analyze_endpoint():
    pdf_file = request.files['text']
    text = text
    topics = articleTopicAnalizi(text)
    return jsonify({topics})
    # return jsonify({"topics": topics})

if __name__ == '__main__':
    app.run(host='localhost', port=7010)

def pdfCozumle(pdf_path):
    doc = fitz.open(pdf_path)
    full_text = ""
    for i in range(min(2, len(doc))): 
        full_text += doc[i].get_text()

    lines = full_text.splitlines()
    abstract, keywords = "", ""
    capture_abstract = False

    for i, line in enumerate(lines):
        if "abstract" in line.lower():
            capture_abstract = True
            continue

        if capture_abstract:
            if line.strip() == "" or "index terms" in line.lower() or "keywords" in line.lower():
                capture_abstract = False
            else:
                abstract += line.strip() + " "

        if "index terms" in line.lower() or "keywords" in line.lower():
            keyword_line = line.lower().replace("index terms—", "").replace("keywords—", "").replace("keywords:", "")
            keywords = keyword_line.strip()
            if i + 1 < len(lines) and not lines[i + 1].strip().endswith('.'):
                keywords += " " + lines[i + 1].strip()

    return abstract.strip(), keywords.strip()
