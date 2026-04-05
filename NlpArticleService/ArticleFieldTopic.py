
import spacy
from collections import Counter

nlp = spacy.load("en_core_web_lg")

fieldTopicDesc = {
    "Deep Learning": "Deep learning utilizes CNN, RNN, LSTM, and Transformers for complex pattern recognition.",
    "Natural Language Processing": "Natural language processing with GPT, BERT, transformers to analyze and generate text.",
    "Computer Vision": "Computer vision includes object detection, segmentation, and analysis of images or videos.",
    "Generative AI": "Generative AI creates synthetic content using GANs, diffusion models, and generative methods.",
    "Brain-Computer Interfaces (BCI)": "Brain-computer interfaces analyze EEG signals, brain activity, emotions, mental states.",
    "User Experience Design": "Improving digital products through user-centered methods, usability, and interface design.",
    "Augmented and Virtual Reality (AR/VR)": "Creating immersive virtual or augmented environments via specialized devices.",
    "Encryption Algorithms": "Securing data using RSA, AES, hashing, and cryptographic encryption methods.",
    "Secure Software Development": "Developing secure software through secure coding, code auditing, and vulnerability management.",
    "Network Security": "Protecting networks from cyberattacks using firewalls, intrusion detection, cybersecurity protocols.",
    "Authentication Systems": "Confirming identities via authentication, biometrics, or multi-factor verification.",
    "Digital Forensics": "Recovering and analyzing digital evidence to investigate cyber incidents and ensure integrity.",
    "5G and Next-Generation Networks": "Providing high-speed connectivity and low latency via 5G technologies, MIMO, beamforming.",
    "Cloud Computing": "Delivering scalable services and infrastructure online via AWS, Azure, Google Cloud.",
    "Blockchain Technology": "Creating secure digital ledgers with blockchain technology, smart contracts, and decentralized consensus.",
    "P2P and Decentralized Systems": "Distributed processing and resource sharing without central servers or authorities.",
    "Data Mining": "Data mining discovers hidden structures using clustering, market-basket analysis, frequent itemset mining, and rule-based prediction methods.",
    "Data Visualization": "Presenting data visually through charts, graphs, dashboards, and visual analytics techniques.",
    "Data Processing Systems": "Managing large-scale data using Hadoop, Spark, ETL, and big data technologies.",
    "Time Series Analysis": "Forecasting temporal data using ARIMA, seasonal decomposition, and predictive modeling."
}

fieldTopicKey = {
    "Deep Learning": ["cnn", "rnn", "lstm", "transformer", "deep", "neural","deep learning", "ai", "ml", "machine learning", "feedforward", "backpropagation", "autoencoder"],
    "Natural Language Processing": ["text", "language", "bert", "gpt", "transformer", "nlp","chatbot", "text mining", "language model", "word2vec", "sentence", "token", "tokenization"],
    "Computer Vision": ["image", "video", "visual object detection","graph neural network", "object", "detection", "segmentation","feature extraction", "face recognition", "image classification", "pattern", "object tracking"],
    "Generative AI": ["gan", "generative", "synthetic", "diffusion","diffusion model", "text-to-image", "deepfake", "generation", "vae"],
    "Brain-Computer Interfaces (BCI)": ["eeg", "brain", "signal", "emotion", "bci", "neural", "fmri","eeg signal", "neurofeedback", "brainwave", "neuro", "stimulus", "erp"],
    "User Experience Design": ["ux", "usability", "interface", "interaction","user-centered", "ux design", "usability test", "human factors", "user interface"],
    "Augmented and Virtual Reality (AR/VR)": ["vr", "ar", "virtual", "augmented", "reality", "immersive","xr", "mixed reality", "vr headset", "oculus", "virtual environment"],
    "Encryption Algorithms": ["aes", "rsa", "hash", "encryption", "cryptographic","cipher", "crypto", "public key", "private key"],
    "Secure Software Development": ["secure", "vulnerability", "threat", "coding","security flaw", "code analysis", "input validation", "secure design"],
    "Network Security": ["network", "security", "firewall", "intrusion", "cyberattack","ddos", "malware", "ids", "vpn", "tcp/ip", "tls"],
    "Authentication Systems": ["authentication", "biometric", "identity", "password", "verification","otp", "two-factor", "identity management", "access control"],
    "Digital Forensics": ["forensics", "incident", "investigation", "evidence","e-discovery", "disk image", "log analysis", "file recovery", "chain of custody"],
    "5G and Next-Generation Networks": ["5g", "latency", "network", "mimo", "beamforming","nr", "millimeter wave", "edge computing"],
    "Cloud Computing": ["cloud", "aws", "azure", "scalable", "computing","virtual machine", "saas", "paas", "iaas", "cloud storage", "cloud service"],
    "Blockchain Technology": ["blockchain", "ledger", "smart contract", "decentralized", "consensus","smart contracts", "nft", "crypto", "token", "distributed ledger"],
    "P2P and Decentralized Systems": ["peer-to-peer", "p2p", "decentralized", "distributed","peer", "torrent", "node", "dht", "p2p network"],
    "Data Mining": ["clustering", "association rules", "market-basket", "itemset", "frequent pattern","pattern recognition", "rule mining", "decision tree", "association mining"],
    "Data Visualization": ["visualization", "chart", "line graph", "bar chart", "dashboard", "visual analytics", "visual interface","plot", "infographic", "data chart", "heatmap", "scatter"],
    "Data Processing Systems": ["hadoop", "spark", "pipeline", "processing","data pipeline", "batch", "stream", "etl", "mapreduce"],
    "Time Series Analysis": ["time series", "temporal", "forecast", "seasonal", "arima", "trend","timeseries", "trend analysis", "forecasting", "lstm", "periodicity"]
}

def anlamliKelimelerBul(text, nlp):
    metin = nlp(text.lower())
    kelimeler = [token.lemma_ for token in metin if token.pos_ in ("NOUN", "PROPN")
             and not token.is_stop and token.has_vector and len(token.lemma_) > 2]
    return Counter(kelimeler)

def enCokGecenKelimeler(dict, n=20):
    return [word for word, count in dict.most_common(n)]

def konulariTespiti(bulunanKelimeler, fieldTopicDesc, nlp, fieldTopicKey):
    kelimeMetni = nlp(" ".join(bulunanKelimeler))
    skor = {}

    for topic, desc in fieldTopicDesc.items():
        desc_doc = nlp(desc)
        semantikSkor = kelimeMetni.similarity(desc_doc)

        kelimeEslesmesi = sum([1 for word in fieldTopicKey.get(topic, []) if word in bulunanKelimeler])
        kelimeSkoru = kelimeEslesmesi  / len(fieldTopicKey.get(topic, [1]))

        toplamSkor = (0.65 * semantikSkor) + (0.35 * kelimeSkoru)
        skor[topic] = toplamSkor

    makaleKonuAnaliz = sorted(skor.items(), key=lambda x: x[1], reverse=True)
    return makaleKonuAnaliz[:4]

def articleTopicAnalizi(text):
    anlamliKelimeler = anlamliKelimelerBul(text, nlp)
    enCokGecenKelime = enCokGecenKelimeler(anlamliKelimeler, n=20)
    bulunanKonular = konulariTespiti(enCokGecenKelime , fieldTopicDesc, nlp, fieldTopicKey)

    return {
        "konular": [{"topic": t, "score": round(s, 2)} for t, s in bulunanKonular]
    }

