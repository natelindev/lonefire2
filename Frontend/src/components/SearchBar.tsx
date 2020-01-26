import * as React from 'react';
import { InputGroup, Input, InputGroupAddon, InputGroupText } from 'reactstrap';
import './SearchBar.scoped.scss';

export default class SearchBar extends React.PureComponent<{}> {
  public render() {
    return (
      <InputGroup>
        <Input placeholder="搜索..." />
        <InputGroupAddon addonType="append">
          <InputGroupText>
            <i className="material-icons">search</i>
          </InputGroupText>
        </InputGroupAddon>
      </InputGroup>
    );
  }
}
