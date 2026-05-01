import { useMemo } from 'react';
import { normalizeText } from '../utils/stringUtils';

export const useMenuFilter = (menuItems, searchText, selectedCategory) => {
  const filteredMenu = useMemo(() => {
    return menuItems.filter(item => {
      const matchSearch = normalizeText(item.name)
        .includes(normalizeText(searchText));

      const matchCategory =
        selectedCategory === 'All' ||
        item.categoryId === selectedCategory;

      return matchSearch && matchCategory;
    });
  }, [menuItems, searchText, selectedCategory]);

  return filteredMenu;
};