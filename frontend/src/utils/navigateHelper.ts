let navigator: ((path: string) => void) | null = null;

/**
 * Set the navigator function (useNavigate) from React Router.
 */
export function setNavigator(nav: (path: string) => void) {
  navigator = nav;
}

/**
 * Use the navigator function to navigate anywhere (even outside React components).
 */
export function navigate(path: string) {
  if (navigator) {
    navigator(path);
  } else {
    console.warn("Navigator is not set yet!");
  }
}
