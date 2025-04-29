# שלב 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

# שלב 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# מעתיקים את תוצרי הבנייה והמסד
COPY --from=build /app/out .
COPY users.db .

# 📦 מתקינים תלויות (כולל OCR ותמיכה בלינוקס)
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
    # ✅ יצירת symlink כדי ש-Tesseract ימצא את הספריה הנכונה
    ln -s /usr/lib/x86_64-linux-gnu/liblept.so.5 /usr/lib/x86_64-linux-gnu/libleptonica-1.80.0.so && \
    apt-get clean

# 📌 חובה ללינוקס: תמיכה ב־System.Drawing
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# ✨ הרצת האפליקציה
ENTRYPOINT ["dotnet", "BusuMatchProject.dll"]
