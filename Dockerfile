# שלב 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

# שלב 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 💾 מעתיקים את הקבצים שנבנו
COPY --from=build /app/out .
COPY users.db .

# 📦 מתקינים ספריות דרושות לתמיכה ב-Tesseract ו-PDF
RUN apt-get update && \
    apt-get install -y \
    libgdiplus \
    libc6-dev \
    tesseract-ocr \
    ghostscript \
    poppler-utils \
    && apt-get clean

# 📌 מפעילים תמיכה מלאה ב־System.Drawing
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# ✨ מריצים את האפליקציה
ENTRYPOINT ["dotnet", "BusuMatchProject.dll"]
