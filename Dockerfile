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

# 📦 מתקינים את כל התלויות הדרושות לפעולה של Pdfium ו־Tesseract
RUN apt-get update && \
    apt-get install -y \
    libgdiplus \
    libc6-dev \
    wget \
    poppler-utils \
    ghostscript \
    && apt-get clean

# 📥 אם ההורדה דרך URL של Pdfium נכשלת, נוודא ש־Pdfium יותקן אוטומטית
RUN wget https://github.com/bblanchon/pdfium-binaries/releases/download/chromium%2F6026/pdfium-linux-x64.tgz -O /tmp/pdfium-linux-x64.tgz || \
    (echo "הורדה נכשלת, מנסה שוב..." && wget https://github.com/bblanchon/pdfium-binaries/releases/download/chromium%2F6026/pdfium-linux-x64.tgz -O /tmp/pdfium-linux-x64.tgz)

# חצי-גיבוי בהורדה ופתיחה של קובץ Pdfium
RUN tar -xvzf /tmp/pdfium-linux-x64.tgz -C /usr/lib && \
    rm -rf /tmp/pdfium-linux-x64.tgz

# 📌 תמיכה ב-System.Drawing
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# ✨ מריצים את האפליקציה
ENTRYPOINT ["dotnet", "BusuMatchProject.dll"]
