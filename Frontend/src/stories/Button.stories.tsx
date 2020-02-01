import React from 'react';
import 'bootstrap-css-only/css/bootstrap.min.css';
import GradientButton from '../components/GradientButton';
export default { title: 'Button' };

export const gradient = () => (
  <GradientButton colorA="#5CC6EE" colorB="#3232ff">
    Button
  </GradientButton>
);
