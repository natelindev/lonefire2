import * as React from 'react';
import {
  Collapse,
  Navbar,
  NavbarBrand,
  NavItem,
  NavLink,
  Nav,
  UncontrolledDropdown,
  DropdownToggle,
  DropdownMenu,
  DropdownItem,
  NavbarToggler
} from 'reactstrap';
import Logo from './Logo';
import { Link } from 'react-router-dom';
import './Navbar.scoped.scss';
import SearchBar from './SearchBar';

export default class NavMenu extends React.PureComponent<{}, { isOpen: boolean }> {
  public state = {
    isOpen: false
  };

  public render() {
    const { isOpen } = this.state;
    return (
      <Navbar color="light" light expand="md">
        <Logo />
        <NavbarBrand tag={Link} to="/">
          Nathaniel's Dev Area
        </NavbarBrand>
        <Collapse isOpen={isOpen} navbar>
          <Nav>
            <NavItem>
              <NavLink>作品集</NavLink>
            </NavItem>
            <NavItem>
              <NavLink>论文</NavLink>
            </NavItem>
          </Nav>
        </Collapse>
        <Collapse isOpen={isOpen} navbar>
          <Nav>
            <NavItem>
              <NavLink>动态</NavLink>
            </NavItem>
            <NavItem>
              <NavLink>关于</NavLink>
            </NavItem>
            <UncontrolledDropdown nav inNavbar>
              <DropdownToggle nav caret>
                更多
              </DropdownToggle>
              <DropdownMenu right>
                <DropdownItem>友链</DropdownItem>
                <DropdownItem>留言板</DropdownItem>
                <DropdownItem>时间线</DropdownItem>
                <DropdownItem>管理</DropdownItem>
              </DropdownMenu>
            </UncontrolledDropdown>
            <NavItem>
              <SearchBar />
            </NavItem>
          </Nav>
        </Collapse>
        <NavbarToggler />
      </Navbar>
    );
  }

  private toggle = () => {
    this.setState({
      isOpen: !this.state.isOpen
    });
  };
}
