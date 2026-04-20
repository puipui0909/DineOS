import React, { createContext, useReducer, useState } from 'react';

export const OrderContext = createContext();

const initialState = {
  currentOrder: null,
  currentTableId: null,
  cart: [],
  total: 0,
  loading: false,
  error: null,
};

const orderReducer = (state, action) => {
  switch (action.type) {
    case 'SET_TABLE':
      return { ...state, currentTableId: action.payload };

    case 'SET_ORDER':
      return { ...state, currentOrder: action.payload };

    case 'ADD_ITEM': {
      const existingItem = state.cart.find(
        (item) => item.menuItemId === action.payload.menuItemId
      );
      if (existingItem) {
        return {
          ...state,
          cart: state.cart.map((item) =>
            item.menuItemId === action.payload.menuItemId
              ? { ...item, quantity: item.quantity + action.payload.quantity }
              : item
          ),
        };
      }
      return {
        ...state,
        cart: [...state.cart, action.payload],
      };
    }

    case 'UPDATE_ITEM_QUANTITY': {
      return {
        ...state,
        cart: state.cart.map((item) =>
          item.menuItemId === action.payload.menuItemId
            ? { ...item, quantity: action.payload.quantity }
            : item
        ),
      };
    }

    case 'REMOVE_ITEM':
      return {
        ...state,
        cart: state.cart.filter((item) => item.menuItemId !== action.payload),
      };

    case 'CLEAR_CART':
      return { ...state, cart: [], currentOrder: null, currentTableId: null };

    case 'SET_LOADING':
      return { ...state, loading: action.payload };

    case 'SET_ERROR':
      return { ...state, error: action.payload };

    default:
      return state;
  }
};

export const OrderProvider = ({ children }) => {
  const [state, dispatch] = useReducer(orderReducer, initialState);

  const setTableId = (tableId) => {
    dispatch({ type: 'SET_TABLE', payload: tableId });
  };

  const setOrder = (order) => {
    dispatch({ type: 'SET_ORDER', payload: order });
  };

  const addToCart = (item) => {
    dispatch({ type: 'ADD_ITEM', payload: item });
  };

  const updateItemQuantity = (menuItemId, quantity) => {
    dispatch({
      type: 'UPDATE_ITEM_QUANTITY',
      payload: { menuItemId, quantity },
    });
  };

  const removeFromCart = (menuItemId) => {
    dispatch({ type: 'REMOVE_ITEM', payload: menuItemId });
  };

  const clearCart = () => {
    dispatch({ type: 'CLEAR_CART' });
  };

  const calculateTotal = () => {
    return state.cart.reduce(
      (sum, item) => sum + item.price * item.quantity,
      0
    );
  };

  return (
    <OrderContext.Provider
      value={{
        ...state,
        total: calculateTotal(),
        setTableId,
        setOrder,
        addToCart,
        updateItemQuantity,
        removeFromCart,
        clearCart,
        dispatch,
      }}
    >
      {children}
    </OrderContext.Provider>
  );
};