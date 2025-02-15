import React from "react";
import { Weather } from "./Weather";

export type SessionUser = {
  userName?: string;
  isAuthenticated: boolean;  
}

export const App: React.FC = () => {
  // State
  const [isLoading, setIsLoading] = React.useState(true);
  const [loadingError, setLoadingError] = React.useState<string | null>(null);
  const [sessionUser, setSessionUser] = React.useState<SessionUser>({ isAuthenticated: false });

  // Check if the user is authenticated
  React.useEffect(() => {
      // Self-invoking async function to check if the user is authenticated
      (async () => {
          try {
            const response = await fetch("/api/user");
            if (!response.ok) {
              throw new Error(`HTTP error! status: ${response.status}`);
            }
            const user: SessionUser = await response.json();
            setSessionUser(user);
            setIsLoading(false);
          } catch (error) {
              setLoadingError((error as Error).message);
              setIsLoading(false);
          }
      })();
  }, []);

  // Display the loading messate
  if (isLoading) {
      return "Loading...";
  }

  // Display the error message
  if (loadingError) {
      return `Error: ${loadingError}`;
  }

  // Redirect the user to the login page if they are not authenticated
  if (sessionUser.isAuthenticated) {
      return <Weather sessionUser={sessionUser} />;
  } else {
      window.location.href = "microsoftidentity/account/signin?redirectUrl=" + window.location.pathname;
  }
};