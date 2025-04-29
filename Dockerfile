# שלב 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

# שלב 2: Runtime עם Pdfium
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 📁 מעתיקים את הקבצים שקומפילנו
COPY --from=build /app/out .

# 💾 מעתיקים את מסד הנתונים
COPY users.db .

# 📦 מתקינים ספריות תלות של libgdiplus (תמיכה ב־System.Drawing) ו־PDFium
RUN apt-get update && \
    apt-get install -y \
    wget \
    libgdiplus \
    libc6-dev \
    && apt-get clean

# 📥 מוסיפים את pdfium.dll והגרסה הנייטיבית של libpdfium
COPY pdfium.dll .
COPY libpdfium.dll .

# ✨ מריצים את האפליקציה
ENTRYPOINT ["dotnet", "BusuMatchProject.dll"]
