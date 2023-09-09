export interface UserReviewsProps {
    loggedInUserId: string | undefined;
    isLoading: boolean;
    setIsLoading: React.Dispatch<React.SetStateAction<boolean>>;
  }