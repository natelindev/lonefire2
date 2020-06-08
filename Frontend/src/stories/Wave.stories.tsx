import React from 'react';
import Wave from '../components/Wave';

export default { title: 'Wave' };

export const wave = (): React.ReactElement => (
  <Wave>
    <h1>Hello, world!</h1>
    <p>Check out my awesome waves!</p>
  </Wave>
);
