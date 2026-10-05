export const homeMenuItem = {
  roleActionId: 'home',
  parentRoleActionId: null,
  role: 'home',
  title: 'Trang chủ',
  url: '/',
  phanLoai: 'Page',
  level: 0,
  sttSapXep: 0,
  children: []
};

export function normalizeUrl(url) {
  if (!url || url === '#') {
    return null;
  }

  return url.startsWith('/') ? url : `/${url}`;
}

export function findFirstNavigableUrl(item) {
  const ownUrl = normalizeUrl(item.url);

  if (ownUrl) {
    return ownUrl;
  }

  for (const child of item.children) {
    const childUrl = findFirstNavigableUrl(child);

    if (childUrl) {
      return childUrl;
    }
  }

  return null;
}

export function hasActiveChild(item, activePath) {
  return item.children.some(
      (child) =>
          findFirstNavigableUrl(child) === activePath ||
          hasActiveChild(child, activePath)
  );
}

export function flattenMenuItems(items) {
  return items.flatMap((item) => [
    item,
    ...flattenMenuItems(item.children)
  ]);
}