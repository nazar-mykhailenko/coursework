import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAppSelector } from "../../store/hooks";

const ProtectedRoute = () => {
  const { isAuthenticated } = useAppSelector((state) => state.auth);
  const location = useLocation();

  if (!isAuthenticated) {
    // Redirect to login page if not authenticated
    // Save the current location to redirect back after authentication
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  // Render child routes if user is authenticated
  return <Outlet />;
};

export default ProtectedRoute;
