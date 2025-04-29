# שלב 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

# שלב 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 💾 מעתיקים את תוצרי הבנייה והמסד
COPY --from=build /app/out .
COPY users.db .

# 📦 מתקינים את כל התלויות
RUN apt-get update && \
    apt-get install -y \
    libgdiplus \
    libc6-dev \
    libpng-dev \
    libjpeg-dev \
    liblept5 \
    libleptonica-dev \
    libtesseract-dev \
    tesseract-ocr \
    tesseract-ocr-heb \
    poppler-utils \
    ghostscript && \
    apt-get clean

# 📌 תמיכה ב-System.Drawing
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# ✨ הרצת האפליקציה
ENTRYPOINT ["dotnet", "BusuMatchProject.dll"]
