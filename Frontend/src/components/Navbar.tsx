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
import GradientButton from './GradientButton';

export default () => (
  <Navbar className="" color="light" light expand="md">
    <Logo />
    <NavbarBrand tag={Link} to="/">
      Nathaniel's Dev Area
    </NavbarBrand>
    <Collapse isOpen={false} className="flex-grow-0" navbar>
      <Nav>
        <NavItem>
          <NavLink>作品集</NavLink>
        </NavItem>
        <NavItem>
          <NavLink>论文</NavLink>
        </NavItem>
        <NavItem>
          <NavLink>关于</NavLink>
        </NavItem>
        <UncontrolledDropdown nav inNavbar>
          <DropdownToggle nav caret>
            更多
          </DropdownToggle>
          <DropdownMenu right>
            <DropdownItem>动态</DropdownItem>
            <DropdownItem>友链</DropdownItem>
            <DropdownItem>留言板</DropdownItem>
            <DropdownItem>时间线</DropdownItem>
          </DropdownMenu>
        </UncontrolledDropdown>
      </Nav>
    </Collapse>
    <Collapse isOpen={false} navbar className="ml-auto mr-0 justify-content-end flex-grow-1">
      <Nav className="flex-grow-1 justify-content-end">
        <NavItem className="searchBar">
          <SearchBar />
        </NavItem>
        <NavItem>
          <GradientButton className="ml-2" colorA="#5CC6EE" colorB="#3232FF">
            登录
          </GradientButton>
        </NavItem>
      </Nav>
    </Collapse>
    <NavbarToggler className="animated--toggler" />
  </Navbar>
);
