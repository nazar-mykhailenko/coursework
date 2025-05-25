import { createBrowserRouter } from "react-router-dom";
import App from "../App";
import LoginPage from "../components/auth/LoginPage";
import RegisterPage from "../components/auth/RegisterPage";
import ProtectedRoute from "../components/auth/ProtectedRoute";
import PublicOnlyRoute from "../components/auth/PublicOnlyRoute";
import DashboardPage from "../components/dashboard/DashboardPage";
import LocationsList from "../components/locations/LocationsList";
import LocationDetail from "../components/locations/LocationDetail";
import CreateLocation from "../components/locations/CreateLocation";
import EditLocation from "../components/locations/EditLocation";
import SeedingPredictionPage from "../components/predictions/SeedingPredictionPage";
import { CropsList, CreateCrop, EditCrop, CropInfo } from "../components/crops";

const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      // Protected routes
      {
        element: <ProtectedRoute />,
        children: [
          {
            path: "/",
            element: <DashboardPage />,
          },
          // Location routes
          {
            path: "locations",
            element: <LocationsList />,
          },
          {
            path: "locations/new",
            element: <CreateLocation />,
          },
          {
            path: "locations/:id",
            element: <LocationDetail />,
          },
          {
            path: "locations/:id/edit",
            element: <EditLocation />,
          }, // Prediction routes
          {
            path: "predictions/seeding",
            element: <SeedingPredictionPage />,
          },
          // Crop routes
          {
            path: "crops",
            element: <CropsList />,
          },
          {
            path: "crops/create",
            element: <CreateCrop />,
          },
          {
            path: "crops/:id",
            element: <CropInfo />,
          },
          {
            path: "crops/:id/edit",
            element: <EditCrop />,
          },
          // Add other protected routes here
        ],
      },
      // Public routes (only accessible when not logged in)
      {
        element: <PublicOnlyRoute />,
        children: [
          {
            path: "login",
            element: <LoginPage />,
          },
          {
            path: "register",
            element: <RegisterPage />,
          },
        ],
      },
    ],
  },
]);

export default router;
