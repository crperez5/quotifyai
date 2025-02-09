import type React from "react"

interface LoaderProps {
  size?: number
  color?: string
}

export const Loader: React.FC<LoaderProps> = ({ size = 24, color = "#3B5998" }) => {
  return (
    <svg width={size} height={size} viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
      <style>
        {`
          .spinner {
            animation: rotate 1s linear infinite;
            transform-origin: center;
          }
          @keyframes rotate {
            100% {
              transform: rotate(360deg);
            }
          }
        `}
      </style>
      <circle
        className="spinner"
        cx="12"
        cy="12"
        r="10"
        fill="none"
        stroke={color}
        strokeWidth="2"
        strokeDasharray="31.4 31.4"
      />
    </svg>
  )
}

