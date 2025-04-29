# שלב 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

# שלב 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS runtime
WORKDIR /app

# 💾 העתקת האפליקציה והמסד
COPY --from=build /app/out .
COPY users.db .

# 📦 מתקינים תלות בסיסיות
RUN apt-get update && apt-get install -y \
    build-essential \
    wget \
    curl \
    libjpeg-dev \
    libpng-dev \
    libtiff-dev \
    zlib1g-dev \
    libgdiplus \
    ghostscript \
    poppler-utils \
    tesseract-ocr \
    tesseract-ocr-heb \
    libtesseract-dev \
    && apt-get clean

# 📥 הורדה והתקנה ידנית של libleptonica (גרסה 1.80.0)
RUN curl -L -o leptonica.tar.gz http://www.leptonica.org/source/leptonica-1.80.0.tar.gz && \
    tar -xvzf leptonica.tar.gz && \
    cd leptonica-1.80.0 && \
    ./configure && \
    make && \
    make install && \
    ldconfig

# 📌 תמיכה ב-System.Drawing
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# ✨ מריצים את האפליקציה
ENTRYPOINT ["dotnet", "BusuMatchProject.dll"]
