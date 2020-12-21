import React from 'react'
import '../styles/styles.css'

export const Toggle = React.forwardRef(({ children, onClick }, ref) => (
    <a
      href=""
      className="threedots float-right mr-3"
      ref={ref}
      onClick={e => {
        e.preventDefault()
        onClick(e)
      }}
    >
      {children}
    </a>
  ))
  