import { useMemo } from 'react';

export const useMenuFilter = (menuItems, searchText, selectedCategory) => {
  const filteredMenu = useMemo(() => {
    return menuItems.filter(item => {
      const matchSearch = item.name
        .toLowerCase()
        .includes(searchText.toLowerCase());

      const matchCategory =
        selectedCategory === 'All' ||
        item.categoryId === selectedCategory;

      return matchSearch && matchCategory;
    });
  }, [menuItems, searchText, selectedCategory]);

  return filteredMenu;
};