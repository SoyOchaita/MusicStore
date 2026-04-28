/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.{cshtml,html}",
    "./Areas/**/*.{cshtml,html}",
    "./Pages/**/*.{cshtml,html}",
    "./wwwroot/js/**/*.{js,ts}"
  ],
  theme: { extend: {} },
  plugins: []
};