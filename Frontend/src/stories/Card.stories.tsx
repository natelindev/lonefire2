import React from 'react';
import Card from '../components/Card';
import { CardTitle, CardText, CardSubtitle, Button, CardBody } from 'reactstrap';
import CardImg from '../components/CardImg';
export default { title: 'Card' };

export const top = () => (
  <Card style={{ maxWidth: '200px' }}>
    <CardImg top width="100%" src="https://placeholder.pics/svg/200" alt="Card image cap" />
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

export const left = () => (
  <Card style={{ maxWidth: '200px' }}>
    <CardImg left width="100%" src="https://placeholder.pics/svg/200" alt="Card image cap" />
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
