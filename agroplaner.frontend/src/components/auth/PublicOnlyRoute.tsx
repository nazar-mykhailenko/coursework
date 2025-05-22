import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAppSelector } from "../../store/hooks";

const PublicOnlyRoute = () => {
  const { isAuthenticated } = useAppSelector((state) => state.auth);
  const location = useLocation();

  // Get the location user was trying to navigate to before being redirected to login
  const from = location.state?.from?.pathname || "/";

  if (isAuthenticated) {
    // If user is authenticated, redirect to the home page or previous attempted location
    return <Navigate to={from} replace />;
  }

  // Render child routes if user is not authenticated
  return <Outlet />;
};

export default PublicOnlyRoute;
