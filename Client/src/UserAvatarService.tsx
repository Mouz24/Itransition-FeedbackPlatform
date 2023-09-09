import { Avatar } from "@mui/material";
import { User } from "./Entities";
import { UserContext } from "./UserContext";
import { deepPurple } from "@mui/material/colors";

export const getAvatarContent = (user: User | UserContext | null) => {
    if (user?.avatar) {
      return <Avatar src={user.avatar} />;
    } else if (user?.username) {
      const firstLetter = user.username.charAt(0).toUpperCase();
      return (
        <Avatar sx={{ bgcolor: deepPurple[500] }}>{firstLetter}</Avatar>
      );
    }
  };