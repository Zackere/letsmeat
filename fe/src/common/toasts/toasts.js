export const success = (text, provider) => {
    provider.add(content('Success', text), {
        appearance: 'success',
        autoDismiss: true,
      })
}

export const error = (text, provider) => {
    provider.add(content('Error', text), {
        appearance: 'error',
        autoDismiss: true,
      })
}

export const warn = (text, provider) => {
    provider.add(content('Warning', text), {
        appearance: 'warning',
        autoDismiss: true,
      })
}

export const info = (text, provider) => {
    provider.add(content('Info', text), {
        appearance: 'info',
        autoDismiss: true,
      })
}


const content = (title, text) => (
    <div>
      <strong>{title}</strong>
      <div>{text}</div>
    </div>
  );