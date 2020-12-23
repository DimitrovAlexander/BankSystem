import styled from "styled-components";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

export const NavContainer = styled.div`
  position: relative;
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  box-shadow: 1px 1px 20px;
  background-image: linear-gradient(to right, #384062, cyan);
  height: 80px;
`;

export const DropdownContainer = styled.div`
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  position: absolute;
  z-index: 1;
  overflow: hidden;
  background-color: white;
  top: 0;
  height: ${(props) => (props.active ? "100vh" : "0vh")};
  transition: height 1s ease;
  width: 100vw;
`;

export const DropdownItemIcon = styled(FontAwesomeIcon)`
  font-size: 3rem;
  margin: 10px;
`;
export const DropdownItemHeading = styled.p`
  font-size: 3rem;
  margin: 10px;
`;

export const DropdownItem = styled.div`
  z-index: 0;
  box-shadow: 1px 5px 10px;
  margin: auto;
  display: flex;
  flex-direction: row;
`;

export const BurgerButton = styled.img`
  position: absolute;
  z-index: 2;
  height: 80px;
`;
