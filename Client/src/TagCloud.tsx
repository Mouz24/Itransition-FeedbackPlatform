import React, { useEffect, useState } from 'react';
import { TagCloud } from 'react-tagcloud';
import { Tag } from './Entities';
import axiosInstance from './AxiosInstance';
import { Box, Chip, Typography } from '@mui/material';

interface TagCloudProps {
  onSelectTag: (selectedTag: Tag) => void;
  selectedTags: Tag[];
  handleRemoveTag: (tagId: number) => void;
}

const TagCloudComponent: React.FC<TagCloudProps> = ({ onSelectTag, selectedTags, handleRemoveTag }) => {
  const [tags, setTags] = useState<Tag[]>([]);

  useEffect(() => {
    fetchTags();
  }, []);

  const fetchTags = async () => {
    const fetchedTags = await axiosInstance.get<Tag[]>('tag');
    setTags(fetchedTags.data);
  }

  const handleTagClick = (tag: Tag ) => {
    onSelectTag(tag);
  };

  return (
    <div>
      <TagCloud
        tags={tags}
        onClick={handleTagClick}
        minSize={12}
        maxSize={35}
        colorOptions={{ luminosity: 'dark' }}
      />
      <Box mt={2}>
            <Typography variant="subtitle1">Selected Tags:</Typography>
            {selectedTags.map((tag) => (
              <Chip
              key={tag.id}
              label={tag.text}
              onDelete={() => handleRemoveTag(tag.id)}
              color="primary"
              variant="outlined"
              style={{ marginRight: '8px', marginBottom: '8px' }}
            />
            ))}
          </Box>
    </div>
  );
};

export default TagCloudComponent;
