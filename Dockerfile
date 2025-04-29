# שלב 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

# שלב 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS runtime
WORKDIR /app

# מעתיקים את תוצרי הבנייה והמסד
COPY --from=build /app/out .
COPY users.db .

# מתקינים את כל התלויות - הכל ביחד בתוך RUN אחד
RUN apt-get update && apt-get install -y \
    tesseract-ocr \
    tesseract-ocr-heb \
    libtesseract-dev \
    libleptonica-dev \
    libjpeg-dev \
    libpng-dev \
    libgdiplus \
    ghostscript \
    poppler-utils \
    libc6-dev && apt-get clean

# תמיכה ב-System.Drawing
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# הרצת האפליקציה
ENTRYPOINT ["dotnet", "BusuMatchProject.dll"]
