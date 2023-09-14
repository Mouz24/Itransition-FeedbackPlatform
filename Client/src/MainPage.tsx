import React, { useState } from 'react';
import { Link, Route, Routes, useNavigate } from 'react-router-dom';
import { Grid, Typography, Box, Avatar, Rating, Chip } from '@mui/material'; // Import Material-UI components
import AllReviews from './AllReviews';
import HighestMarkedReviews from './HighestMarkedReviews';
import TagCloud from './TagCloud';
import { useUserContext } from './UserContext';
import { Review, Tag } from './Entities';
import axiosInstance from './AxiosInstance';
// import SearchBar from './SearchBar';
import UserReviews from './UserReviews';import signalRArtworkService from './SignalRArtworkService';
import ReviewItem from './ReviewItem';
;

const MainPage: React.FC = () => {
  const [selectedTags, setSelectedTags] = useState<Tag[]>([]);
  const [selectedTagsIds, setSelectedTagsIds] = useState<number[]>([]);
  const [searchResults, setSearchResults] = useState<Review[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [searched, setSearched] = useState(false);
  const { loggedInUser } = useUserContext();

  const handleTagSelection = (selectedTag: Tag) => {
    if (!selectedTags.includes(selectedTag)) {
      setSelectedTags([...selectedTags, selectedTag]);
      setSelectedTagsIds(getSelectedTagsIds());
    }
  };

  const getSelectedTagsIds = () => {
    return selectedTags.map((tag) => tag.id);
  }

  const handleRemoveTag = (tagToRemove : number) => {
    const updatedTags = selectedTags.filter((tag) => tag.id !== tagToRemove);
    setSelectedTags(updatedTags);
  };

  const handleSearch = async (searchTerm: string) => {
    if (searchTerm.length === 0) {
      setSearched(false);
    } else {
      const response = await axiosInstance.get<Review[]>(`search?word=${searchTerm}`);
      setSearchResults(response.data);
      setSearched(true);
    }
  };

  return (
    <div>
      <Grid container spacing={2}>
        <Grid item xs={12} sm={4}>
          <TagCloud onSelectTag={handleTagSelection} selectedTags={selectedTags} handleRemoveTag={handleRemoveTag}/>
        </Grid>

        <Grid item xs={12} sm={8}>
          {searched ? (
            searchResults.length === 0 ? (
              <p>No results found.</p>
            ) : (
              <div>
                {searchResults.map((review) => (
                  <ReviewItem review={review} loggedInUserId={loggedInUser?.id} />    
              ))}
              </div>
            )
          ) : (
            <Box sx={{marginTop: '10px'}}>
              <Typography fontFamily={'monospace'} variant='h5'>All Reviews</Typography>
              <AllReviews isLoading={isLoading} setIsLoading={setIsLoading} loggedInUserId={loggedInUser?.id} tagIds={selectedTagsIds} />
              <Typography fontFamily={'monospace'} variant='h5'>Highest Marked Reviews</Typography>
              <HighestMarkedReviews isLoading={isLoading} setIsLoading={setIsLoading} loggedInUserId={loggedInUser?.id} tagIds={selectedTagsIds} />
            </Box>
          )}
        </Grid>
      </Grid>
    </div>
  );
};

export default MainPage;
