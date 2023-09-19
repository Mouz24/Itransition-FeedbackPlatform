export interface ReviewsProps {
    goal: string;
    loggedInUserId: string | undefined;
    tagIds: number[];
    isLoading: boolean;
    setIsLoading: React.Dispatch<React.SetStateAction<boolean>>;
  }