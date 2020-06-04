import React from 'react';

interface MyProps {
  label: string;
}

export function MyComponent(props: MyProps): React.ReactElement {
  const a = 'hi';
  props.label = a;
  label = a;
  return <p>{label}</p>;
}
