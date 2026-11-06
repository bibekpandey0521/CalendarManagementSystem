/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./app/**/*.{js,ts,jsx,tsx}",
    "./components/**/*.{js,ts,jsx,tsx}",
    "./node_modules/@fullcalendar/react/*.{js,ts,jsx,tsx}",
    "./node_modules/@fullcalendar/daygrid/*.{js,ts,jsx,tsx}",
    "./node_modules/@fullcalendar/timegrid/*.{js,ts,jsx,tsx}",
    "./node_modules/@fullcalendar/list/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ["Inter", "system-ui", "sans-serif"],
        display: ["Geist", "sans-serif"],
      },
    },
  },
  plugins: [],
};