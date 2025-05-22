import { Outlet } from "react-router-dom";
import { useAppSelector } from "./store/hooks";
import LogoutButton from "./components/auth/LogoutButton";
import "./App.css";

function App() {
  const { isAuthenticated } = useAppSelector((state) => state.auth);

  return (
    <div className="app-container">
      <header className="app-header">
        <h1>AgroPlaner</h1>
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
