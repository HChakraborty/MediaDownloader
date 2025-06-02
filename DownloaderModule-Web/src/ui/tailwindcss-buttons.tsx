"use client";

import { useState } from "react";

const buttonLabels = [
  "Explore",
  "Photos",
  "Illustrations",
  "Vectors",
  "Videos",
  "Music",
  "Sound Effects",
  "GIFs",
];

export default function ButtonRow() {
  const [active, setActive] = useState("Explore");

  return (
    <div className="flex justify-center flex-wrap gap-x-6 gap-y-3 mt-4 mb-8" style={{marginTop: 40, marginBottom: 20}}>
      {buttonLabels.map((label) => (
        <button
          key={label}
          onClick={() => setActive(label)}
          className={`text-lg font-medium transition-all duration-200
            ${
              label === active
                ? "bg-white text-black px-5 py-2 rounded-full shadow"
                : "text-white hover:text-gray-300"
            }`} style={{paddingLeft: 16, paddingRight: 16, paddingTop: 7, paddingBottom: 7}}
        >
          {label}
        </button>
      ))}
    </div>
  );
}
