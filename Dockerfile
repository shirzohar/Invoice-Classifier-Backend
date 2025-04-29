# שלב 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

# שלב 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 💾 מעתיקים את הקבצים שנבנו
COPY --from=build /app/out .
COPY users.db .

# 📦 מתקינים את כל התלויות הדרושות ל־Tesseract ולעבודה עם תמונות
RUN apt-get update && \
    apt-get install -y \
    libgdiplus \
    libc6-dev \
    libpng-dev \
    libjpeg-dev \
    poppler-utils \
    ghostscript \
    tesseract-ocr \
    tesseract-ocr-heb && \
    apt-get clean

# 📌 תמיכה ב-System.Drawing
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# ✨ מריצים את האפליקציה
ENTRYPOINT ["dotnet", "BusuMatchProject.dll"]
