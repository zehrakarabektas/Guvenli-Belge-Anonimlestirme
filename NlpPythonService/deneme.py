import spacy
from flask import Flask,request,jsonify

app=Flask(__name__)
nlp=spacy.load("en_core_web_md")
fieldTopicKey={
    "Derin öðrenme":["deep learning","cnn","lstm","rnn","autoencoder","mlp","gan","netural network","vae","backpropagation", "dropout", "overfitting", 
                     "training data", "feature extraction", "learning rate", "epoch", "batch size"],

    "Doðal dil iþleme":["nlp","text classification","bert","gpt","tokenization","named entity recognition", "pos tagging", "dependency parsing", 
                        "word embeddings", "word2vec", "transformer model", "seq2seq", "attention mechanism", "text mining","natural language processing","language model"],

    "Bilgisayarda görü":["yolo","opencv","image classification","semantic segmentation", "instance segmentation", "image captioning", "visual recognition", "feature map", 
                         "object tracking", "region proposal", "resnet", "vgg","computer vision","object detection","image processing"],

    "Generatif yapay zeka":["generative ai","gpt","gan","stable diffusion","diffusion models", "text-to-image", "style transfer", "image synthesis", "text generation", "vae", 
                            "dreambooth", "prompt engineering", "image generation","text generation"],

    "Beyin-bilgisayar arayüzleri (BCI)":["bci","brain computer interface", "eeg", "erp", "fnirs", "brain computer interface", "neural decoding", "motor imagery", "eeg classification", 
                                         "brain signal processing", "brain signal", "neurofeedback", "p300", "ssvep"],

    "Kullanýcý deneyimi tasarýmý":["ux","user testing","interface design","usability","user experience", "cognitive load", "interaction design", "wireframe", "information architecture",
                                  "personas", "usability testing", "accessibility", "human centered design"],

    "Arttýrýlmýþ ve sanal gerçeklik (AR/VR)":["ar","vr","xr","mixed reality", "augmented reality", "haptic", "oculus", "unity3d", "unreal engine", "head-mounted display", "3d modeling", 
                                              "immersive experience", "virtual environment", "depth sensing","virtual reality","3d interaction"],

    "Þifreleme algoritmalarý":["cryptography", "sha","encryption", "rsa", "aes", "public key", "private key", "diffie hellman", "zero knowledge proof", "homomorphic encryption",
                              "elliptic curve", "hashing", "digital signature"],

    "Güvenli yazýlým geliþtirme":["secure coding", "code analysis", "owasp", "static analysis", "dynamic analysis", "threat modeling", "secure development lifecycle", "code injection", 
                                  "cross site scripting", "sql injection", "static code analysis", "input validation", "buffer overflow"],

    "Að güvenliði":["network security", "firewall", "ids", "ips", "packet sniffer", "network traffic analysis", "port scanning", "honeypot", "ip spoofing", "man in the middle",
                    "vpn", "tls", "ssl", "port scanning"],

    "Kimlik doðrulama sistemleri":["authentication", "authorization", "oauth", "biometric login","single sign on", "access token", "session management", "biometric authentication",
                                  "passwordless login", "multi-factor authentication", "2fa"],

    "Adli biliþim":["digital forensics", "log analysis", "file carving", "evidence preservation", "forensic imaging", "chain of custody", "metadata analysis", "data leakage", 
                    "log forensics", "incident response", "malware analysis", "data recovery"],

    "5G ve yeni nesil aðlar":["5g", "6g", "edge computing", "nr", "mmwave", "massive mimo", "beamforming", "ultra reliable low latency", "urllc", "network latency", "network densification",
                              "latency", "network slicing"],

    "Bulut biliþim":["cloud computing", "aws", "azure", "cloud storage", "iaas", "paas", "saas", "virtual machine", "cloud architecture", "cloud platform", "serverless", "kubernetes",
                     "gcp", "cloud service", "cloud infrastructure"],

    "Blockchain teknolojisi":["blockchain", "smart contract", "ethereum","hyperledger", "mining", "block size", "proof of work", "proof of stake", "dapp", "wallet", "token", "ledger",
                              "distributed ledger", "consensus", "crypto"],

    "P2P ve merkeziyetsiz sistemler":["p2p", "peer to peer", "decentralized", "torrent", "node", "gossip protocol", "swarm", "trustless", "overlay network", "consensusless", 
                                      "self organizing system", "ipfs", "dht", "distributed system", "federated"],

    "Veri madenciliði":["data mining", "clustering",  "decision tree", "unsupervised learning", "supervised learning", "pattern recognition", "outlier detection", "feature selection", 
                        "classification algorithm","classification", "association rule", "frequent pattern", "k-means", "apriori"],

    "Veri görselleþtirme":["data visualization", "bar chart", "heatmap","data dashboard", "data storytelling", "treemap", "sunburst chart", "interactive visualization", "seaborn", 
                           "viz", "pivot chart", "plotly", "matplotlib", "dashboard", "graph", "line chart", "histogram", "chart"],

    "Veri iþleme sistemleri":["hadoop", "spark",  "dataflow", "distributed processing", "storm", "kafka", "real-time processing", "batch processing", "data ingestion", 
                              "distributed file system", "dfs", "hdfs", "mapreduce", "hive", "flink", "etl", "data pipeline"],

    "Zaman serisi analizi":["time series", "forecasting", "rolling average", "arima", "lstm", "trend forecasting", "lag", "moving average", "windowing", "autocorrelation", "seasonal adjustment",
                           "stationarity", "tsfresh", "trend analysis", "seasonality",  "temporal data", "sensor data"],

    }
fieldTopicDesc = {
    "Derin öðrenme": "Deep learning refers to neural networks such as CNN, LSTM and GAN that learn hierarchical features from large datasets.",
    "Doðal dil iþleme": "Natural language processing focuses on understanding and generating human language using models like BERT and GPT.",
    "Bilgisayarda görü": "Computer vision enables machines to interpret and analyze visual information like images and videos using object detection.",
    "Generatif yapay zeka": "Generative AI generates content like images or text using models such as GANs or diffusion-based architectures.",
    "Beyin-bilgisayar arayüzleri (BCI)": "BCI systems process EEG signals to detect user intention or emotional state without physical interaction.",
    "Kullanýcý deneyimi tasarýmý": "UX design improves product usability and accessibility through human-centered design and user testing.",
    "Arttýrýlmýþ ve sanal gerçeklik (AR/VR)": "AR and VR technologies create immersive digital environments for enhanced visual experiences and interaction.",
    "Þifreleme algoritmalarý": "Cryptographic algorithms secure data using encryption, hashing, digital signatures, and public-key methods.",
    "Güvenli yazýlým geliþtirme": "Secure coding practices prevent software vulnerabilities such as SQL injection and buffer overflow.",
    "Að güvenliði": "Network security involves protecting communication systems using firewalls, IDS/IPS, and traffic analysis.",
    "Kimlik doðrulama sistemleri": "Authentication systems verify user identity using passwords, tokens, biometrics, or multi-factor methods.",
    "Adli biliþim": "Digital forensics investigates cyber incidents using log analysis, data recovery, and evidence preservation.",
    "5G ve yeni nesil aðlar": "5G networks enable ultra-low latency and high bandwidth using technologies like beamforming and network slicing.",
    "Bulut biliþim": "Cloud computing offers scalable resources and services via platforms like AWS, Azure, and GCP.",
    "Blockchain teknolojisi": "Blockchain ensures decentralized trust with smart contracts, distributed ledgers and consensus algorithms.",
    "P2P ve merkeziyetsiz sistemler": "P2P systems share resources directly without central authority using decentralized protocols.",
    "Veri madenciliði": "Data mining extracts patterns from data using clustering, decision trees and frequent itemset algorithms.",
    "Veri görselleþtirme": "Data visualization represents data visually using charts, graphs and dashboards for easier interpretation.",
    "Veri iþleme sistemleri": "Big data systems like Spark and Hadoop handle large-scale processing in batch and real-time pipelines.",
    "Zaman serisi analizi": "Time series analysis models temporal patterns using techniques like ARIMA, LSTM and trend forecasting."
}
def analyze_text(text):
    text_doc = nlp(text)
    text_lower = text.lower()

    results = []

    for topic, desc in fieldTopicDesc.items():
        desc_doc = nlp(desc)
        similarity_score = text_doc.similarity(desc_doc)

        keyword_count = sum(kw.lower() in text_lower for kw in fieldTopicKey[topic])
        keyword_score = min(keyword_count / 5, 1.0) 

        total_score = round(0.7 * similarity_score + 0.3 * keyword_score, 4)

        results.append({
            "topic": topic,
            "similarity": round(similarity_score, 4),
            "keyword_score": keyword_score,
            "total_score": total_score
        })

    # En yüksek 3 sonucu döndür
    return sorted(results, key=lambda x: x["total_score"], reverse=True)[:3]
if __name__ == "__main__":
    sample_text = """
    As an important task in the advanced stage of artificial intelligence, the research of emotional EEG has received 
    more and more attention in recent years. In order to improve the accuracy of EEG signal emotion recognition, 
    Fast Fourier Transform (FFT) and Continuous Wavelet Transform (CWT) are used to extract the features of EEG signals 
    and build CNN models for emotion classification. The DEAP dataset is used in our experiments.
    """
    
    results = analyze_text(sample_text)

    for r in results:
        print(f"{r['topic']}: Total={r['total_score']}, Similarity={r['similarity']}, Keyword Score={r['keyword_score']}")
# import spacy

# # Daha iyi sonuç için en_core_web_md kullan (pip install & download gerekir)
# nlp = spacy.load("en_core_web_md")

# # Konu açýklamalarý
# topic_descriptions = {
#     "Derin öðrenme": "Deep learning refers to neural networks such as CNN, LSTM and GAN that learn hierarchical features from large datasets.",
#     "Doðal dil iþleme": "Natural language processing focuses on understanding and generating human language using models like BERT and GPT.",
#     "Bilgisayarda görü": "Computer vision enables machines to interpret and analyze visual information like images and videos using object detection.",
#     "Generatif yapay zeka": "Generative AI generates content like images or text using models such as GANs or diffusion-based architectures.",
#     "Beyin-bilgisayar arayüzleri (BCI)": "BCI systems process EEG signals to detect user intention or emotional state without physical interaction.",
#     "Kullanýcý deneyimi tasarýmý": "UX design improves product usability and accessibility through human-centered design and user testing.",
#     "Arttýrýlmýþ ve sanal gerçeklik (AR/VR)": "AR and VR technologies create immersive digital environments for enhanced visual experiences and interaction.",
#     "Þifreleme algoritmalarý": "Cryptographic algorithms secure data using encryption, hashing, digital signatures, and public-key methods.",
#     "Güvenli yazýlým geliþtirme": "Secure coding practices prevent software vulnerabilities such as SQL injection and buffer overflow.",
#     "Að güvenliði": "Network security involves protecting communication systems using firewalls, IDS/IPS, and traffic analysis.",
#     "Kimlik doðrulama sistemleri": "Authentication systems verify user identity using passwords, tokens, biometrics, or multi-factor methods.",
#     "Adli biliþim": "Digital forensics investigates cyber incidents using log analysis, data recovery, and evidence preservation.",
#     "5G ve yeni nesil aðlar": "5G networks enable ultra-low latency and high bandwidth using technologies like beamforming and network slicing.",
#     "Bulut biliþim": "Cloud computing offers scalable resources and services via platforms like AWS, Azure, and GCP.",
#     "Blockchain teknolojisi": "Blockchain ensures decentralized trust with smart contracts, distributed ledgers and consensus algorithms.",
#     "P2P ve merkeziyetsiz sistemler": "P2P systems share resources directly without central authority using decentralized protocols.",
#     "Veri madenciliði": "Data mining extracts patterns from data using clustering, decision trees and frequent itemset algorithms.",
#     "Veri görselleþtirme": "Data visualization represents data visually using charts, graphs and dashboards for easier interpretation.",
#     "Veri iþleme sistemleri": "Big data systems like Spark and Hadoop handle large-scale processing in batch and real-time pipelines.",
#     "Zaman serisi analizi": "Time series analysis models temporal patterns using techniques like ARIMA, LSTM and trend forecasting."
# }

# # Makale metni
# text = """
# As an important task in the advanced stage of 
# artificial intelligence, the research of emotional EEG has received 
# more and more attention in recent years. In order to improve the 
# accuracy of EEG signal emotion recognition, in this paper, Fast 
# Fourier Transform (FFT) and Continuous Wavelet Transform 
# (CWT) are used to extract the features of EEG signals on the 
# DEAP data set and build two CNN models for emotion 
# recognition. The results show that the proposed algorithm is 
# effective for EEG signal emotion recognition. The average 
# recognition accuracy of emotion valence can reach 75.9%; the 
# arousal can reach 79.3%; the like/dislike can reach 80.7%. This 
# research can provide practical application reference for 
# continuous dimension emotion automatic analysis and machine 
# recognition. EEG; FFT; CWT; CNN; emotion 
# recognition
# """

# # Analiz
# doc_input = nlp(text)
# for topic, desc in topic_descriptions.items():
#     sim = doc_input.similarity(nlp(desc))
#     print(f"{topic}: {sim:.3f}")

# input("\nPress Enter to exit...")
# import spacy
# from flask import Flask,request,jsonify

# app=Flask(__name__)
# nlp=spacy.load("en_core_web_md")
# fieldTopicKey={
#     "Derin öðrenme":["deep learning","cnn","lstm","rnn","autoencoder","mlp","gan","netural network","vae","backpropagation", "dropout", "overfitting", 
#                      "training data", "feature extraction", "learning rate", "epoch", "batch size"],

#     "Doðal dil iþleme":["nlp","text classification","bert","gpt","tokenization","named entity recognition", "pos tagging", "dependency parsing", 
#                         "word embeddings", "word2vec", "transformer model", "seq2seq", "attention mechanism", "text mining","natural language processing","language model"],

#     "Bilgisayarda görü":["yolo","opencv","image classification","semantic segmentation", "instance segmentation", "image captioning", "visual recognition", "feature map", 
#                          "object tracking", "region proposal", "resnet", "vgg","computer vision","object detection","image processing"],

#     "Generatif yapay zeka":["generative ai","gpt","gan","stable diffusion","diffusion models", "text-to-image", "style transfer", "image synthesis", "text generation", "vae", 
#                             "dreambooth", "prompt engineering", "image generation","text generation"],

#     "Beyin-bilgisayar arayüzleri (BCI)":["bci","brain computer interface", "eeg", "erp", "fnirs", "brain computer interface", "neural decoding", "motor imagery", "eeg classification", 
#                                          "brain signal processing", "brain signal", "neurofeedback", "p300", "ssvep"],

#     "Kullanýcý deneyimi tasarýmý":["ux","user testing","interface design","usability","user experience", "cognitive load", "interaction design", "wireframe", "information architecture",
#                                   "personas", "usability testing", "accessibility", "human centered design"],

#     "Arttýrýlmýþ ve sanal gerçeklik (AR/VR)":["ar","vr","xr","mixed reality", "augmented reality", "haptic", "oculus", "unity3d", "unreal engine", "head-mounted display", "3d modeling", 
#                                               "immersive experience", "virtual environment", "depth sensing","virtual reality","3d interaction"],

#     "Þifreleme algoritmalarý":["cryptography", "sha","encryption", "rsa", "aes", "public key", "private key", "diffie hellman", "zero knowledge proof", "homomorphic encryption",
#                               "elliptic curve", "hashing", "digital signature"],

#     "Güvenli yazýlým geliþtirme":["secure coding", "code analysis", "owasp", "static analysis", "dynamic analysis", "threat modeling", "secure development lifecycle", "code injection", 
#                                   "cross site scripting", "sql injection", "static code analysis", "input validation", "buffer overflow"],

#     "Að güvenliði":["network security", "firewall", "ids", "ips", "packet sniffer", "network traffic analysis", "port scanning", "honeypot", "ip spoofing", "man in the middle",
#                     "vpn", "tls", "ssl", "port scanning"],

#     "Kimlik doðrulama sistemleri":["authentication", "authorization", "oauth", "biometric login","single sign on", "access token", "session management", "biometric authentication",
#                                   "passwordless login", "multi-factor authentication", "2fa"],

#     "Adli biliþim":["digital forensics", "log analysis", "file carving", "evidence preservation", "forensic imaging", "chain of custody", "metadata analysis", "data leakage", 
#                     "log forensics", "incident response", "malware analysis", "data recovery"],

#     "5G ve yeni nesil aðlar":["5g", "6g", "edge computing", "nr", "mmwave", "massive mimo", "beamforming", "ultra reliable low latency", "urllc", "network latency", "network densification",
#                               "latency", "network slicing"],

#     "Bulut biliþim":["cloud computing", "aws", "azure", "cloud storage", "iaas", "paas", "saas", "virtual machine", "cloud architecture", "cloud platform", "serverless", "kubernetes",
#                      "gcp", "cloud service", "cloud infrastructure"],

#     "Blockchain teknolojisi":["blockchain", "smart contract", "ethereum","hyperledger", "mining", "block size", "proof of work", "proof of stake", "dapp", "wallet", "token", "ledger",
#                               "distributed ledger", "consensus", "crypto"],

#     "P2P ve merkeziyetsiz sistemler":["p2p", "peer to peer", "decentralized", "torrent", "node", "gossip protocol", "swarm", "trustless", "overlay network", "consensusless", 
#                                       "self organizing system", "ipfs", "dht", "distributed system", "federated"],

#     "Veri madenciliði":["data mining", "clustering",  "decision tree", "unsupervised learning", "supervised learning", "pattern recognition", "outlier detection", "feature selection", 
#                         "classification algorithm","classification", "association rule", "frequent pattern", "k-means", "apriori"],

#     "Veri görselleþtirme":["data visualization", "bar chart", "heatmap","data dashboard", "data storytelling", "treemap", "sunburst chart", "interactive visualization", "seaborn", 
#                            "viz", "pivot chart", "plotly", "matplotlib", "dashboard", "graph", "line chart", "histogram", "chart"],

#     "Veri iþleme sistemleri":["hadoop", "spark",  "dataflow", "distributed processing", "storm", "kafka", "real-time processing", "batch processing", "data ingestion", 
#                               "distributed file system", "dfs", "hdfs", "mapreduce", "hive", "flink", "etl", "data pipeline"],

#     "Zaman serisi analizi":["time series", "forecasting", "rolling average", "arima", "lstm", "trend forecasting", "lag", "moving average", "windowing", "autocorrelation", "seasonal adjustment",
#                            "stationarity", "tsfresh", "trend analysis", "seasonality",  "temporal data", "sensor data"],

#     }
# fieldTopicDesc = {
#     "Derin öðrenme": "Deep learning involves hierarchical representation learning using neural networks like CNNs, LSTMs, and GANs for high-dimensional data analysis.",
#     "Doðal dil iþleme": "Natural language processing enables machines to understand, interpret, and generate human language using models such as BERT and GPT.",
#     "Bilgisayarda görü": "Computer vision analyzes visual inputs like images and videos through techniques such as object detection and image segmentation.",
#     "Generatif yapay zeka": "Generative AI creates new content such as images, text, or audio using models like GANs, diffusion models, or transformers.",
#     "Beyin-bilgisayar arayüzleri (BCI)": "Brain-computer interfaces decode brain signals like EEG to enable control of external devices or emotion recognition without physical movement.",
#     "Kullanýcý deneyimi tasarýmý": "User experience design focuses on optimizing system usability, accessibility, and satisfaction through iterative and user-centered approaches.",
#     "Arttýrýlmýþ ve sanal gerçeklik (AR/VR)": "AR/VR systems deliver immersive environments by overlaying or simulating 3D content via head-mounted displays and spatial tracking.",
#     "Þifreleme algoritmalarý": "Cryptographic algorithms secure data transmission and storage through encryption, hashing, and digital signature mechanisms.",
#     "Güvenli yazýlým geliþtirme": "Secure software development applies practices to prevent vulnerabilities like injection attacks, insecure code, and buffer overflows.",
#     "Að güvenliði": "Network security defends data transmission from attacks through firewalls, intrusion detection systems, and secure protocols.",
#     "Kimlik doðrulama sistemleri": "Authentication systems verify digital identity using credentials like passwords, tokens, biometrics, or multi-factor techniques.",
#     "Adli biliþim": "Digital forensics investigates cyber incidents by collecting, preserving, and analyzing digital evidence from devices and networks.",
#     "5G ve yeni nesil aðlar": "5G networks support ultra-reliable low-latency communication using technologies like network slicing and massive MIMO.",
#     "Bulut biliþim": "Cloud computing delivers scalable IT resources such as storage, compute, and services over the internet via platforms like AWS or Azure.",
#     "Blockchain teknolojisi": "Blockchain is a decentralized ledger technology that enables secure and tamper-proof transactions using consensus mechanisms.",
#     "P2P ve merkeziyetsiz sistemler": "Peer-to-peer and decentralized systems operate without central authority, enabling direct resource sharing and distributed control.",
#     "Veri madenciliði": "Data mining uncovers hidden patterns and relationships in large datasets using algorithms like clustering, classification, and association rules.",
#     "Veri görselleþtirme": "Data visualization translates data into graphical formats like charts and plots to support interpretation and decision-making.",
#     "Veri iþleme sistemleri": "Big data processing systems like Spark and Hadoop perform distributed computation on large-scale datasets in batch or real time.",
#     "Zaman serisi analizi": "Time series analysis models temporal patterns using techniques like ARIMA, LSTM, and seasonal decomposition."
# }

# def analyze_text(text):
#     text_doc = nlp(text)
#     text_lower = text.lower()

#     results = []

#     for topic, desc in fieldTopicDesc.items():
#         desc_doc = nlp(desc)
#         similarity_score = text_doc.similarity(desc_doc)

#         keyword_count = sum(kw.lower() in text_lower for kw in fieldTopicKey[topic])
#         keyword_score = min(keyword_count / 5, 1.0) 

#         total_score = round(0.7 * similarity_score + 0.3 * keyword_score, 4)

#         results.append({
#             "topic": topic,
#             "similarity": round(similarity_score, 4),
#             "keyword_score": keyword_score,
#             "total_score": total_score
#         })

#     return sorted(results, key=lambda x: x["total_score"], reverse=True)[:]
# def compute_keyword_score(text, keywords):
#     doc = nlp(text.lower())
#     tokens = [token.lemma_ for token in doc if not token.is_stop and not token.is_punct]

#     match_count = 0
#     for kw in keywords:
#         kw_doc = nlp(kw)
#         for token in tokens:
#             token_doc = nlp(token)
#             # Tam eþleþme veya semantik benzerlik > 0.85
#             if kw == token or (kw_doc.vector_norm and token_doc.vector_norm and kw_doc.similarity(token_doc) > 0.85):
#                 match_count += 1
#                 break  # Ayný keyword tekrar sayýlmasýn

#     return min(match_count / 5, 1.0)  # Normalize et (maks 1.0)

# # Ana analiz fonksiyonu
# def analyze_text(text):
#     text_doc = nlp(text)
#     results = []

#     for topic, desc in fieldTopicDesc.items():
#         desc_doc = nlp(desc)
#         similarity_score = text_doc.similarity(desc_doc)
#         keyword_score = compute_keyword_score(text, fieldTopicKey[topic])

#         # Aðýrlýklar: %90 semantik, %10 keyword
#         total_score = round(0.95 * similarity_score + 0.5 * keyword_score, 4)

#         results.append({
#             "topic": topic,
#             "similarity": round(similarity_score, 4),
#             "keyword_score": round(keyword_score, 4),
#             "total_score": total_score
#         })

#     return sorted(results, key=lambda x: x["total_score"], reverse=True)



# if __name__ == "__main__":
#     sample_text = """
#  ABSTRACT Emotion recognition using EEG signals is an emerging area of research due to its broad
#  applicability in Brain-Computer Interfaces. Emotional feelings are hard to stimulate in the lab. Emotions
#  don’t last long, yet they need enough context to be perceived and felt. However, most EEG-related
#  emotion databases either suffer from emotionally irrelevant details (due to prolonged duration stimulus)
#  or have minimal context, which may not elicit enough emotion. We tried to overcome this problem by
#  designing an experiment in which participants were free to report their emotional feelings while watching
#  the emotional stimulus. We called these reported emotional feelings ‘‘Emotional Events’’ in our Dataset
#  on Emotion with Naturalistic Stimuli (DENS), which has the recorded EEG signals during the emotional
#  events. To compare our dataset, we classify emotional events on different combinations of Valence(V) and
#  Arousal(A) dimensions and compared the results with benchmark datasets of DEAP and SEED. Short
# Time Fourier Transform (STFT) is used for feature extraction and in the classification model consisting of
#  CNN-LSTM hybrid layers. We achieved significantly higher accuracy with our data compared to DEAP
#  and SEED data. We conclude that having precise information about emotional feelings improves the
#  classification accuracy compared to long-duration recorded EEG signals which might be contaminated by
#  mind-wandering. This dataset can be used for detailed analysis of specific experienced emotions and related
#  brain dynamics.
#  INDEX TERMS Affective computing, CNN, DEAP, DENS, EEG, emotion dataset, emotion recognition,
#  LSTM, SEED"""
    
#     results = analyze_text(sample_text)

#     for r in results:
#         print(f"{r['topic']}: Total={r['total_score']}, Similarity={r['similarity']}, Keyword Score={r['keyword_score']}")
# import spacy
# from collections import Counter

# nlp = spacy.load("en_core_web_md")

# topics = {
#     "Derin öðrenme": "Deep learning utilizes hierarchical neural network architectures like CNNs, RNNs, and Transformers to perform complex pattern recognition tasks.",
    
#     "Doðal dil iþleme": "Natural language processing focuses on interpreting and generating human languages through models like GPT, BERT, and transformer-based architectures.",
    
#     "Bilgisayarda görü": "Computer vision involves analyzing visual data to identify objects and features using image recognition, segmentation, and classification methods.",
    
#     "Generatif yapay zeka": "Generative AI creates original synthetic content such as images, text, or audio using generative models like GANs and diffusion methods.",
    
#     "Beyin-bilgisayar arayüzleri (BCI)": "Brain-computer interfaces interpret neural signals, particularly EEG and fMRI, to control devices or decode mental states and emotions.",
    
#     "Kullanýcý deneyimi tasarýmý": "User experience design improves the usability and interaction quality of digital products through user-centered design processes and usability evaluations.",
    
#     "Arttýrýlmýþ ve sanal gerçeklik (AR/VR)": "Augmented and virtual reality technologies create immersive environments or overlays using specialized headsets and interactive 3D content.",
    
#     "Þifreleme algoritmalarý": "Encryption algorithms secure information confidentiality using cryptographic methods such as RSA, AES, and hashing techniques.",
    
#     "Güvenli yazýlým geliþtirme": "Secure software development ensures robust software by preventing vulnerabilities through secure coding practices, threat modeling, and code analysis.",
    
#     "Að güvenliði": "Network security protects network infrastructures from unauthorized access, cyberattacks, and intrusions using firewalls, IDS, and secure protocols.",
    
#     "Kimlik doðrulama sistemleri": "Authentication systems reliably verify user identities through methods including passwords, biometrics, or multi-factor authentication.",
    
#     "Adli biliþim": "Digital forensics involves recovering and investigating digital evidence to analyze cyber incidents and preserve evidence integrity.",
    
#     "5G ve yeni nesil aðlar": "5G and next-generation networks provide high-speed, low-latency connectivity through technologies like massive MIMO, beamforming, and network slicing.",
    
#     "Bulut biliþim": "Cloud computing delivers scalable computing resources and services over the internet via platforms such as AWS, Azure, and Google Cloud.",
    
#     "Blockchain teknolojisi": "Blockchain technology creates secure and tamper-proof digital ledgers using decentralized consensus mechanisms and smart contracts.",
    
#     "P2P ve merkeziyetsiz sistemler": "Peer-to-peer and decentralized systems enable resource sharing and distributed processing without central servers or authorities.",
    
#     "Veri madenciliði": "Data mining involves discovering hidden patterns and relationships within datasets using methods like clustering, classification, and association rule learning.",
    
#     "Veri görselleþtirme": "Data visualization converts complex datasets into visual representations such as charts, graphs, and dashboards to facilitate analysis and interpretation.",
    
#     "Veri iþleme sistemleri": "Data processing systems manage and analyze large-scale data efficiently using frameworks like Hadoop, Spark, or real-time data pipelines.",
    
#     "Zaman serisi analizi": "Time series analysis statistically models, analyzes, and forecasts sequential temporal data using methods like ARIMA, SARIMA, and seasonal decomposition."
# }


# def get_meaningful_words(text, nlp):
#     doc = nlp(text.lower())
#     words = [token.lemma_ for token in doc if token.pos_ in ("NOUN", "PROPN") and not token.is_stop and token.has_vector and len(token.lemma_) > 2]
#     return Counter(words)

# def get_top_meaningful_words(freq_dict, n=20):
#     return [word for word, count in freq_dict.most_common(n)]

# def semantic_similarity_topics(top_words, topics, nlp):
#     top_words_doc = nlp(" ".join(top_words))
#     scores = {}
#     for topic, desc in topics.items():
#         desc_doc = nlp(desc)
#         similarity = top_words_doc.similarity(desc_doc)
#         if similarity > 0:
#             scores[topic] = similarity
#     sorted_scores = sorted(scores.items(), key=lambda x: x[1], reverse=True)
#     return sorted_scores


# # Main Function (using text variable directly)
# if __name__ == '__main__':
 #    text = """ ABSTRACT Emotion recognition using EEG signals is an emerging area of research due to its broad
 #  applicability in Brain-Computer Interfaces. Emotional feelings are hard to stimulate in the lab. Emotions
 #  don’t last long, yet they need enough context to be perceived and felt. However, most EEG-related
 #  emotion databases either suffer from emotionally irrelevant details (due to prolonged duration stimulus)
 #  or have minimal context, which may not elicit enough emotion. We tried to overcome this problem by
 #  designing an experiment in which participants were free to report their emotional feelings while watching
 #  the emotional stimulus. We called these reported emotional feelings ‘‘Emotional Events’’ in our Dataset
 #  on Emotion with Naturalistic Stimuli (DENS), which has the recorded EEG signals during the emotional
 #  events. To compare our dataset, we classify emotional events on different combinations of Valence(V) and
 #  Arousal(A) dimensions and compared the results with benchmark datasets of DEAP and SEED. Short
 # Time Fourier Transform (STFT) is used for feature extraction and in the classification model consisting of
 #  CNN-LSTM hybrid layers. We achieved significantly higher accuracy with our data compared to DEAP
 #  and SEED data. We conclude that having precise information about emotional feelings improves the
 #  classification accuracy compared to long-duration recorded EEG signals which might be contaminated by
 #  mind-wandering. This dataset can be used for detailed analysis of specific experienced emotions and related
 #  brain dynamics.
 #  INDEX TERMS Affective computing, CNN, DEAP, DENS, EEG, emotion dataset, emotion recognition,
 #  LSTM, SEED"""
# freq_dict = get_meaningful_words(text, nlp)
# top_meaningful_words = get_top_meaningful_words(freq_dict, n=20)

# if top_meaningful_words:
#     sorted_topics = semantic_similarity_topics(top_meaningful_words, topics, nlp)

#     print(f"Analyzed meaningful words: {top_meaningful_words}")
#     print("Detected Topics (sorted by confidence):")
#     for topic, score in sorted_topics:
#         print(f"- {topic}: {score:.2f}")
# else:
#     print("Metinde anlamlý kelime bulunamadý.")



# import spacy
# from collections import Counter

# nlp = spacy.load("en_core_web_md")

# topics = {
#     "Derin öðrenme": "Deep learning utilizes hierarchical neural network architectures like CNNs, RNNs, and Transformers to perform complex pattern recognition tasks.",
#     "Doðal dil iþleme": "Natural language processing focuses on interpreting and generating human languages through models like GPT, BERT, and transformer-based architectures.",
#     "Bilgisayarda görü": "Computer vision involves analyzing visual data to identify objects and features using image recognition, segmentation, and classification methods.",
#     "Generatif yapay zeka": "Generative AI creates original synthetic content such as images, text, or audio using generative models like GANs and diffusion methods.",
#     "Beyin-bilgisayar arayüzleri (BCI)": "Brain-computer interfaces interpret neural signals, particularly EEG and fMRI, to control devices or decode mental states and emotions.",
#     "Kullanýcý deneyimi tasarýmý": "User experience design improves the usability and interaction quality of digital products through user-centered design processes and usability evaluations.",
#     "Arttýrýlmýþ ve sanal gerçeklik (AR/VR)": "Augmented and virtual reality technologies create immersive environments or overlays using specialized headsets and interactive 3D content.",
#     "Þifreleme algoritmalarý": "Encryption algorithms secure information confidentiality using cryptographic methods such as RSA, AES, and hashing techniques.",
#     "Güvenli yazýlým geliþtirme": "Secure software development ensures robust software by preventing vulnerabilities through secure coding practices, threat modeling, and code analysis.",
#     "Að güvenliði": "Network security protects network infrastructures from unauthorized access, cyberattacks, and intrusions using firewalls, IDS, and secure protocols.",
#     "Kimlik doðrulama sistemleri": "Authentication systems reliably verify user identities through methods including passwords, biometrics, or multi-factor authentication.",
#     "Adli biliþim": "Digital forensics involves recovering and investigating digital evidence to analyze cyber incidents and preserve evidence integrity.",
#     "5G ve yeni nesil aðlar": "5G and next-generation networks provide high-speed, low-latency connectivity through technologies like massive MIMO, beamforming, and network slicing.",
#     "Bulut biliþim": "Cloud computing delivers scalable computing resources and services over the internet via platforms such as AWS, Azure, and Google Cloud.",
#     "Blockchain teknolojisi": "Blockchain technology creates secure and tamper-proof digital ledgers using decentralized consensus mechanisms and smart contracts.",
#     "P2P ve merkeziyetsiz sistemler": "Peer-to-peer and decentralized systems enable resource sharing and distributed processing without central servers or authorities.",
#     "Veri madenciliði": "Data mining involves discovering hidden patterns and relationships within datasets using methods like clustering, classification, and association rule learning.",
#     "Veri görselleþtirme": "Data visualization converts complex datasets into visual representations such as charts, graphs, and dashboards to facilitate analysis and interpretation.",
#     "Veri iþleme sistemleri": "Data processing systems manage and analyze large-scale data efficiently using frameworks like Hadoop, Spark, or real-time data pipelines.",
#     "Zaman serisi analizi": "Time series analysis statistically models, analyzes, and forecasts sequential temporal data using methods like ARIMA, SARIMA, and seasonal decomposition."
# }

# bonus_keywords = {
#     "Derin öðrenme": ["cnn", "rnn", "lstm", "transformer", "deep", "neural", "network"],
#     "Doðal dil iþleme": ["text", "language", "bert", "gpt", "transformer", "nlp"],
#     "Bilgisayarda görü": ["image", "video", "visual", "object", "detection", "segmentation"],
#     "Generatif yapay zeka": ["gan", "generative", "synthetic", "diffusion"],
#     "Beyin-bilgisayar arayüzleri (BCI)": ["eeg", "brain", "signal", "emotion", "bci", "neural", "fmri"],
#     "Kullanýcý deneyimi tasarýmý": ["ux", "usability", "interface", "interaction"],
#     "Arttýrýlmýþ ve sanal gerçeklik (AR/VR)": ["vr", "ar", "virtual", "augmented", "reality", "immersive"],
#     "Þifreleme algoritmalarý": ["aes", "rsa", "hash", "encryption", "cryptographic"],
#     "Güvenli yazýlým geliþtirme": ["secure", "vulnerability", "threat", "coding", "safe"],
#     "Að güvenliði": ["network", "security", "firewall", "intrusion", "cyberattack"],
#     "Kimlik doðrulama sistemleri": ["authentication", "biometric", "identity", "password", "verification"],
#     "Adli biliþim": ["forensics", "incident", "investigation", "evidence"],
#     "5G ve yeni nesil aðlar": ["5g", "latency", "network", "mimo", "beamforming"],
#     "Bulut biliþim": ["cloud", "aws", "azure", "scalable", "computing"],
#     "Blockchain teknolojisi": ["blockchain", "ledger", "smart contract", "decentralized", "consensus"],
#     "P2P ve merkeziyetsiz sistemler": ["peer", "p2p", "decentralized", "distributed"],
#     "Veri madenciliði": ["mining", "pattern", "clustering", "classification", "association"],
#     "Veri görselleþtirme": ["visualization", "chart", "graph", "dashboard", "visual"],
#     "Veri iþleme sistemleri": ["hadoop", "spark", "pipeline", "processing", "big data"],
#     "Zaman serisi analizi": ["time series", "temporal", "forecast", "seasonal", "arima", "trend"]
# }

# # Fonksiyonlar:
# def get_meaningful_words(text, nlp):
#     doc = nlp(text.lower())
#     words = [token.lemma_ for token in doc if token.pos_ in ("NOUN", "PROPN")
#              and not token.is_stop and token.has_vector and len(token.lemma_) > 2]
#     return Counter(words)

# def get_top_meaningful_words(freq_dict, n=20):
#     return [word for word, count in freq_dict.most_common(n)]

# def semantic_similarity_with_bonus(top_words, topics, nlp, bonus_keywords):
#     top_words_doc = nlp(" ".join(top_words))
#     scores = {}

#     for topic, desc in topics.items():
#         desc_doc = nlp(desc)
#         similarity = top_words_doc.similarity(desc_doc)

#         # Anahtar kelime bonusu ekle
#         keyword_bonus = sum([0.05 for word in bonus_keywords.get(topic, []) if word in top_words])

#         total_score = similarity + keyword_bonus
#         scores[topic] = total_score

#     sorted_scores = sorted(scores.items(), key=lambda x: x[1], reverse=True)
#     return sorted_scores

# # Kullanýmý:
# text = """Abstract:
# This paper presents a comprehensive approach to enhancing user interaction in augmented reality (AR) environments using deep learning-based computer vision methods. We developed a novel real-time object detection and segmentation pipeline employing convolutional neural networks (CNNs) optimized for AR headset hardware constraints. To validate our model, extensive user experience (UX) studies were conducted, measuring interaction quality and user immersion. Experimental results indicate that integrating deep neural networks significantly improves real-time AR performance and overall user satisfaction. Additionally, cloud-based big data processing frameworks like Apache Spark were leveraged for analyzing large-scale user interaction data, facilitating rapid iterative improvements.

# Keywords:
# Augmented reality, user experience, deep learning, computer vision, object detection, segmentation, CNN, cloud computing, big data, Apache Spark"""

# freq_dict = get_meaningful_words(text, nlp)
# top_meaningful_words = get_top_meaningful_words(freq_dict, n=20)

# sorted_topics = semantic_similarity_with_bonus(top_meaningful_words, topics, nlp, bonus_keywords)

# print(f"Analyzed meaningful words: {top_meaningful_words}")
# print("\nDetected Topics (sorted by improved confidence):")
# for topic, score in sorted_topics:
#     print(f"- {topic}: {score:.2f}")
