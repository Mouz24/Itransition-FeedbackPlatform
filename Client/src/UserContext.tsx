import React, { createContext, useContext, useState, ReactNode, useEffect } from 'react';
import { Avatar } from "@mui/material";
import { User } from "./Entities";
import { deepPurple } from "@mui/material/colors";

export interface UserContext {
  id: string;
  userName: string;
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

export const getAvatarContent = (user: User | UserContext | null) => {
  if (user?.avatar) {
    return <Avatar src={user.avatar} />;
  } else if (user?.userName) {
    const firstLetter = user.userName.charAt(0).toUpperCase();
    return (
      <Avatar sx={{ bgcolor: deepPurple[500] }}>{firstLetter}</Avatar>
    );
  }
};

export const canDoReviewManipulations = (loggedInUser: UserContext | null, userId: string | undefined) => {
  return loggedInUser?.id === userId || loggedInUser?.role === 'Administrator';
}

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
