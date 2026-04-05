import spacy
from collections import Counter

nlp = spacy.load("en_core_web_lg")

fieldTopicDesc = {
    "Derin öğrenme": "Deep learning utilizes CNN, RNN, LSTM, and Transformers for complex pattern recognition.",
    "Doğal dil işleme": "Natural language processing with GPT, BERT, transformers to analyze and generate text.",
    "Bilgisayarda görü": "Computer vision includes object detection, segmentation, and analysis of images or videos.",
    "Generatif yapay zeka": "Generative AI creates synthetic content using GANs, diffusion models, and generative methods.",
    "Beyin-bilgisayar arayüzleri (BCI)": "Brain-computer interfaces analyze EEG signals, brain activity, emotions, mental states.",
    "Kullanıcı deneyimi tasarımı": "Improving digital products through user-centered methods, usability, and interface design.",
    "Arttırılmış ve sanal gerçeklik (AR/VR)": "Creating immersive virtual or augmented environments via specialized devices.",
    "Şifreleme algoritmaları": "Securing data using RSA, AES, hashing, and cryptographic encryption methods.",
    "Güvenli yazılım geliştirme": "Developing secure software through secure coding, code auditing, and vulnerability management.",
    "Ağ güvenliği": "Protecting networks from cyberattacks using firewalls, intrusion detection, cybersecurity protocols.",
    "Kimlik doğrulama sistemleri": "Confirming identities via authentication, biometrics, or multi-factor verification.",
    "Adli bilişim": "Recovering and analyzing digital evidence to investigate cyber incidents and ensure integrity.",
    "5G ve yeni nesil ağlar": "Providing high-speed connectivity and low latency via 5G technologies, MIMO, beamforming.",
    "Bulut bilişim": "Delivering scalable services and infrastructure online via AWS, Azure, Google Cloud.",
    "Blockchain teknolojisi": "Creating secure digital ledgers with blockchain technology, smart contracts, and decentralized consensus.",
    "P2P ve merkeziyetsiz sistemler": "Distributed processing and resource sharing without central servers or authorities.",
    "Veri madenciliği": "Data mining discovers hidden structures using clustering, market-basket analysis, frequent itemset mining and rule-based prediction methods.",
    "Veri görselleştirme": "Presenting data visually through charts, graphs, dashboards, and visual analytics techniques.",
    "Veri işleme sistemleri": "Managing large-scale data using Hadoop, Spark, ETL and big data technologies.",
    "Zaman serisi analizi": "Forecasting temporal data using ARIMA, seasonal decomposition, and predictive modeling."
}

fieldTopicKey = {
    "Derin öğrenme": ["cnn", "rnn", "lstm", "transformer", "deep", "neural"],
    "Doğal dil işleme": ["text", "language", "bert", "gpt", "transformer", "nlp"],
    "Bilgisayarda görü": ["image", "video", "visual", "object", "detection", "segmentation"],
    "Generatif yapay zeka": ["gan", "generative", "synthetic", "diffusion"],
    "Beyin-bilgisayar arayüzleri (BCI)": ["eeg", "brain", "signal", "emotion", "bci", "neural", "fmri"],
    "Kullanıcı deneyimi tasarımı": ["ux", "usability", "interface", "interaction"],
    "Arttırılmış ve sanal gerçeklik (AR/VR)": ["vr", "ar", "virtual", "augmented", "reality", "immersive"],
    "Şifreleme algoritmaları": ["aes", "rsa", "hash", "encryption", "cryptographic"],
    "Güvenli yazılım geliştirme": ["secure", "vulnerability", "threat", "coding"],
    "Ağ güvenliği": ["network", "security", "firewall", "intrusion", "cyberattack"],
    "Kimlik doğrulama sistemleri": ["authentication", "biometric", "identity", "password", "verification"],
    "Adli bilişim": ["forensics", "incident", "investigation", "evidence"],
    "5G ve yeni nesil ağlar": ["5g", "latency", "network", "mimo", "beamforming"],
    "Bulut bilişim": ["cloud", "aws", "azure", "scalable", "computing"],
    "Blockchain teknolojisi": ["blockchain", "ledger", "smart contract", "decentralized", "consensus"],
    "P2P ve merkeziyetsiz sistemler": ["peer-to-peer", "p2p", "decentralized", "distributed"],
    "Veri madenciliği": ["clustering", "association rules", "market-basket", "itemset", "frequent pattern"],
    "Veri görselleştirme": ["visualization", "chart", "graph", "dashboard", "visual"],
    "Veri işleme sistemleri": ["hadoop", "spark", "pipeline", "processing"],
    "Zaman serisi analizi": ["time series", "temporal", "forecast", "seasonal", "arima", "trend"]
}

def anlamliKelimeler(text, nlp):
    doc = nlp(text.lower())
    words = [token.lemma_ for token in doc if token.pos_ in ("NOUN", "PROPN")
             and not token.is_stop and token.has_vector and len(token.lemma_) > 2]
    return Counter(words)

def enCokGecenKelimeler(freq_dict, n=20):
    return [word for word, count in freq_dict.most_common(n)]

def konulariTespiti(top_words, fieldTopicDesc, nlp, fieldTopicKey):
    top_doc = nlp(" ".join(top_words))
    scores = {}

    for topic, desc in fieldTopicDesc.items():
        desc_doc = nlp(desc)
        semantic_score = top_doc.similarity(desc_doc)

        keyword_matches = sum([1 for word in fieldTopicKey.get(topic, []) if word in top_words])
        keyword_ratio = keyword_score = keyword_matches / len(fieldTopicKey.get(topic, [1]))

        total_score = (0.65 * semantic_score) + (0.35 * keyword_score)
        scores[topic] = total_score

    sorted_scores = sorted(scores.items(), key=lambda x: x[1], reverse=True)
    return sorted_scores[:4]

def articleTopicAnalizi(text):
    freq_dict = anlamliKelimeler(text, nlp)
    top_meaningful_words = enCokGecenKelimeler(freq_dict, n=20)
    detected_topics = konulariTespiti(top_meaningful_words, fieldTopicDesc, nlp, fieldTopicKey)

    return {
        "topics": [{"topic": t, "score": round(s, 2)} for t, s in detected_topics]
    }
# @app.route('/articleFieldTopicAnalyze', methods=['POST'])
# def analyze():
#     data = request.json
#     text = data['text']
#     freq_dict = get_meaningful_words(text, nlp)
#     top_meaningful_words = get_top_meaningful_words(freq_dict, n=20)
#     detected_topics = balanced_topic_detection(top_meaningful_words, topics, nlp, bonus_keywords)

#     return jsonify({
#         "topics": [{"topic": t, "score": round(s, 2)} for t, s in detected_topics]
#     })

# if __name__ == '__main__':
#     app.run(host='localhost', port=7057)
