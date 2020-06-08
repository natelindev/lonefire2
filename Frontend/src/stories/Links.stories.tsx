import React from 'react';
import AnimatedLink from '../components/AnimatedLink';

export default { title: 'Links' };

export const animated = (): React.ReactElement => (
  <AnimatedLink href="www.google.com">Google</AnimatedLink>
);
