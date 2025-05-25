import { Outlet, Link } from "react-router-dom";
import { useAppSelector } from "./store/hooks";
import LogoutButton from "./components/auth/LogoutButton";
import "./App.css";

function App() {
  const { isAuthenticated } = useAppSelector((state) => state.auth);

  return (
    <div className="app-container">
      <header className="app-header">
        <div className="app-header-left">
          <h1>AgroPlaner</h1>
        </div>
        {isAuthenticated && (
          <div className="app-header-nav">
            <Link to="/" className="nav-link">
              Dashboard
            </Link>
            <Link to="/locations" className="nav-link">
              Locations
            </Link>
            <Link to="/crops" className="nav-link">
              Crops
            </Link>
          </div>
        )}
        {isAuthenticated && <LogoutButton />}
      </header>

      <main className="app-content">
        <Outlet />
      </main>

      <footer className="app-footer">
        <p>
          &copy; {new Date().getFullYear()} AgroPlaner. All rights reserved.
        </p>
      </footer>
    </div>
  );
}

export default App;
