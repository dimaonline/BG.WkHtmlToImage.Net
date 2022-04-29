.NET wrapper for wkhtmltoimage (html to image converter), working on Linux, macOS, Windows and docker.

    string result = new HtmlToImageConverter().Convert(
    "https://google.com", 
    "/Users/user/Downloads/google.jpg", 
    "--width 1024 --height 768 --quality 75");