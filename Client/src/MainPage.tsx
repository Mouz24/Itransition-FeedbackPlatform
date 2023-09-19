import React, { useEffect, useState } from 'react';
import { Grid, Typography, Box, Avatar, Rating, Chip } from '@mui/material';
import Reviews from './Reviews';
import HighestMarkedReviews from './Reviews';
import TagCloud from './TagCloud';
import { useUserContext } from './UserContext';
import { Review, Tag } from './Entities';
import axiosInstance from './AxiosInstance';
import SearchBar from './SearchBar';
import ReviewItem from './ReviewItem';

const MainPage: React.FC = () => {
  const [selectedTags, setSelectedTags] = useState<Tag[]>([]);
  const [selectedTagsIds, setSelectedTagsIds] = useState<number[]>([]);
  const [searchResults, setSearchResults] = useState<Review[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [searched, setSearched] = useState(false);
  const { loggedInUser } = useUserContext();

  const mainPageClasses = loggedInUser?.isDarkMode ? 'main-page dark-mode' : 'main-page';

  useEffect(() => {
    setSelectedTagsIds(selectedTags.map(tag => tag.id))
  }, [selectedTags]);
  
  const handleTagSelection = (selectedTag: Tag) => {
    const updatedTags = [...selectedTags, selectedTag];
    const updatedTagIds = updatedTags.map(tag => tag.id);
    setSelectedTags(updatedTags);
    setSelectedTagsIds(updatedTagIds);
  };
  
  const handleRemoveTag = (tagToRemove: number) => {
    const updatedTags = selectedTags.filter(tag => tag.id !== tagToRemove);
    const updatedTagIds = updatedTags.map(tag => tag.id);
    setSelectedTags(updatedTags);
    setSelectedTagsIds(updatedTagIds);
  };

  const handleSearch = async (searchTerm: string) => {
    console.log(searchTerm);
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
      <SearchBar onSearch={handleSearch} />
      <Grid container spacing={3} mt={1}>
        <Grid item xs={2}>
          <TagCloud onSelectTag={handleTagSelection} selectedTags={selectedTags} handleRemoveTag={handleRemoveTag}/>
        </Grid>
          {searched ? (
            searchResults.length === 0 ? (
              <p>No results found.</p>
            ) : (
              <div>
                {searchResults.map((review) => (
                  <ReviewItem key={review.id} review={review} loggedInUserId={loggedInUser?.id} />    
              ))}
              </div>
            )
          )
          : (
            <>
              <Grid item xs={5}>
              <Typography fontFamily={'monospace'} variant='h5'>All Reviews</Typography>
              <Reviews goal={'all'} isLoading={isLoading} setIsLoading={setIsLoading} loggedInUserId={loggedInUser?.id} tagIds={selectedTagsIds} />
              </Grid>
              <Grid item xs={5}>
              <Typography fontFamily={'monospace'} variant='h5'>Highest Marked Reviews</Typography>
              <HighestMarkedReviews goal={'highest-marked'} isLoading={isLoading} setIsLoading={setIsLoading} loggedInUserId={loggedInUser?.id} tagIds={selectedTagsIds} />
              </Grid>
            </>
          )}
      </Grid>
    </div>
  );
};

export default MainPage;
