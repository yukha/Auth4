

## Before start on developer machine
### 1. Create docker network App4-net
```
docker network create -d bridge Auth4-net
```
### 2. Generate self signed certificate
```angular2html
openssl req -x509 -newkey rsa:4096 \
 -keyout ./nginx/tls/key.pem \
 -out ./nginx/tls/cert.pem \
 -nodes \
 -subj "/C=CZ/ST=Czech/L=Prague/O=ICZ/CN=localhost" \
 -days 3650 \
 -config <( \
    echo '[req]'; \
    echo 'distinguished_name=req'; \
    echo '[san]'; \
    echo 'subjectAltName=DNS:localhost,IP:127.0.0.1')
```
'-nodes' if you don't want to protect your private key with a passphrase
