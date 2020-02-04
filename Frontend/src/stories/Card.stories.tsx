import React from 'react';
import { Canvas } from 'react-three-fiber';
import 'bootstrap-css-only/css/bootstrap.min.css';
import Card from '../components/Card';
import { CardTitle, CardText, CardSubtitle, Button, CardBody } from 'reactstrap';
import CardImg from '../components/CardImg';
import CardTilt from '../components/CardTilt';
export default { title: 'Card' };

export const top = () => (
  <Card style={{ maxWidth: '300px' }}>
    <CardImg
      style={{ maxWidth: '300px' }}
      top
      src="https://placeholder.pics/svg/300"
      alt="Card image cap"
    />
    <CardBody>
      <CardTitle>Card title</CardTitle>
      <CardSubtitle>Card subtitle</CardSubtitle>
      <CardText>
        Some quick example text to build on the card title and make up the bulk of the card's
        content.
      </CardText>
      <Button>Button</Button>
    </CardBody>
  </Card>
);

export const bottom = () => (
  <Card style={{ maxWidth: '300px' }}>
    <CardBody>
      <CardTitle>Card title</CardTitle>
      <CardSubtitle>Card subtitle</CardSubtitle>
      <CardText>
        Some quick example text to build on the card title and make up the bulk of the card's
        content.
      </CardText>
      <Button>Button</Button>
    </CardBody>
    <CardImg bottom src="https://placeholder.pics/svg/300" alt="Card image cap" />
  </Card>
);

export const left = () => (
  <Card lr>
    <CardImg
      style={{ maxWidth: '300px' }}
      left
      src="https://placeholder.pics/svg/300"
      alt="Card image cap"
    />
    <CardBody>
      <CardTitle>Card title</CardTitle>
      <CardSubtitle>Card subtitle</CardSubtitle>
      <CardText>
        Some quick example text to build on the card title and make up the bulk of the card's
        content.
      </CardText>
      <Button>Button</Button>
    </CardBody>
  </Card>
);

export const right = () => (
  <Card lr>
    <CardBody>
      <CardTitle>Card title</CardTitle>
      <CardSubtitle>Card subtitle</CardSubtitle>
      <CardText>
        Some quick example text to build on the card title and make up the bulk of the card's
        content.
      </CardText>
      <Button>Button</Button>
    </CardBody>
    <CardImg
      style={{ maxWidth: '300px' }}
      right
      src="https://placeholder.pics/svg/300"
      alt="Card image cap"
    />
  </Card>
);

export const bubba = () => (
  <Card hoverEffect="bubba" style={{ maxWidth: '300px' }}>
    <CardImg src="https://placeholder.pics/svg/300" alt="img02">
      <h2 className="title mx-auto">Example card</h2>
      <p className="text">Bubba likes to appear out of thin air.</p>
    </CardImg>
  </Card>
);

export const tilt = () => (
  <Canvas
    camera={{
      fov: 75,
      aspect: window.innerWidth / window.innerHeight,
      near: 0.1,
      far: 1000,
      position: [0, 0, 15]
    }}
  >
    <CardTilt></CardTilt>
  </Canvas>
);

export const box = () => (
  <Canvas>
    <ambientLight />
    <pointLight position={[10, 10, 10]} />
    <CardTilt position={[-1.2, 0, 0]} />
    <CardTilt position={[1.2, 0, 0]} />
  </Canvas>
);
