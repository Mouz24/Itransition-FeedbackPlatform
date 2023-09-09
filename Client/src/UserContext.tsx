import React, { createContext, useContext, useState, ReactNode, useEffect } from 'react';

export interface UserContext {
  id: string;
  username: string;
  role: string;
  avatar: string;
}

interface UserContextType {
  loggedInUser: UserContext | null;
  setLoggedInUser: (loggedInUser: UserContext | null) => void;
}

const UserContext = createContext<UserContextType | undefined>(undefined);

export const useUserContext = () => {
  const context = useContext(UserContext);
  if (!context) {
    throw new Error('useUserContext must be used within a UserProvider');
  }
  return context;
};

export const UserProvider: React.FC<{ children: ReactNode }> = (props) => {
  const [loggedInUser, setLoggedInUser] = useState<UserContext | null>(() => {
    const storedUser = localStorage.getItem('user');
    return storedUser ? JSON.parse(storedUser) : null;
  });

  useEffect(() => {
    if (loggedInUser) {
      localStorage.setItem('user', JSON.stringify(loggedInUser));
    } else {
      localStorage.removeItem('user');
    }
  }, [loggedInUser]);

  return (
    <UserContext.Provider value={{ loggedInUser, setLoggedInUser }}>
      {props.children}
    </UserContext.Provider>
  );
};
