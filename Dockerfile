# שלב build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

# שלב ריצה
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS runtime
WORKDIR /app

# 📦 מתקינים ספריות דרושות
RUN apt-get update && \
    apt-get install -y \
    libgdiplus \
    libc6-dev \
    libpng-dev \
    libjpeg-dev \
    ghostscript \
    poppler-utils \
    wget && \
    # 📥 התקנת PDFium (גרסה מותאמת ל־Linux)
    wget https://github.com/bblanchon/pdfium-binaries/releases/download/chromium%2F6121/pdfium-linux.tgz && \
    tar -xvzf pdfium-linux.tgz && \
    cp pdfium/lib/libpdfium.so /usr/lib/libpdfium.so && \
    rm -rf pdfium* && \
    apt-get clean

COPY --from=build /app/out .

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "BusuMatchProject.dll"]
