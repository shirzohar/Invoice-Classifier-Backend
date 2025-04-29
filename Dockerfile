# שלב 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

# שלב 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/out .
COPY users.db .

# 📦 מתקינים תלויות
RUN apt-get update && \
    apt-get install -y \
    libgdiplus \
    libc6-dev \
    libpng-dev \
    libjpeg-dev \
    liblept5 \
    tesseract-ocr \
    tesseract-ocr-heb \
    poppler-utils \
    ghostscript && \
    # 🛠 תיקון הקריסה של libleptonica
    ln -s /usr/lib/x86_64-linux-gnu/liblept.so.5 /usr/lib/x86_64-linux-gnu/libleptonica-1.80.0.so || true && \
    apt-get clean

# 📌 תמיכה ב-System.Drawing
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# ✨ מריצים את האפליקציה
ENTRYPOINT ["dotnet", "BusuMatchProject.dll"]
