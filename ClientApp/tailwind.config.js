/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["./src/**/*.{js,jsx,ts,tsx}"],
    theme: {
        extend: {
            screens: {
                desktop: "1028px",
            },
            colors: {
                "text-primary":
                    "rgb(var(--color-text-primary) / <alpha-value>)",
                "text-secondary":
                    "rgb(var(--color-text-secondary) / <alpha-value>)",
                "action-button":
                    "rgb(var(--color-action-button) / <alpha-value>)",
            },
        },
    },
    plugins: [],
};
