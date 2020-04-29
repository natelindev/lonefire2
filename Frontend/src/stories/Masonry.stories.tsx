import React from 'react';
import Masonry from 'react-masonry-component';
import { Card } from 'reactstrap';
export default { title: 'Masonry' };

const masonryOptions = {
  transitionDuration: 0,
};

export const masonry = () => {
  const childElements = [
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
    [Math.random() * 300, Math.random() * 300],
  ].map(([height, width], index) => {
    return (
      <li className="card" key={width}>
        <Card style={{ width: `200px`, height: `${height}px` }}>{index}</Card>
      </li>
    );
  });
  return (
    <Masonry
      className={'my-gallery-class'} // default ''
      elementType={'ul'} // default 'div'
      options={masonryOptions} // default {}
      disableImagesLoaded={false} // default false
      updateOnEachImageLoad={false} // default false and works only if disableImagesLoaded is false
    >
      {childElements}
    </Masonry>
  );
};
