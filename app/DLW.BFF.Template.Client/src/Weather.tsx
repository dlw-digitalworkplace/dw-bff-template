import React from "react";
import { SessionUser } from "./App";

export type WeatherProps = {
  sessionUser: SessionUser;
};
export const Weather: React.FC<WeatherProps> = (props: WeatherProps) => {
  // Props
  const { sessionUser } = props;

  // State
  const [isLoading, setIsLoading] = React.useState(true);
  const [loadingError, setLoadingError] = React.useState<string | null>(null);
  const [weatherData, setWeatherData] = React.useState<string[]>([]);

  // Fetch the weather data
  React.useEffect(() => {
    // Self-invoking async function to fetch the weather data
    (async () => {
      try {
        const response = await fetch("/api/weather");
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data: string[] = await response.json();
        setWeatherData(data);
        setIsLoading(false);
      } catch (error) {
        setLoadingError((error as Error).message);
        setIsLoading(false);
      }
    })();
  }, []);

  // Display the loading message
  if (isLoading) {
    return <></>;
  }

  // Display the error message
  if (loadingError) {
    return <div>Error: {loadingError}</div>;
  }

  // Display the weather data
  return (
    <div>
      <h1>Welcome {sessionUser.userName}</h1>
      <h2>Weather Data</h2>
      <ul>
        {weatherData.map((data, index) => (
          <li key={index}>{data}</li>
        ))}
      </ul>
    </div>
  );
};